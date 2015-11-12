using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Transaction
using System.Transactions;

namespace RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes
{
    public interface IEnlistmentNotification
    {
        void Commit(Enlistment enlistment);
        void InDoubt(Enlistment enlistment);
        void Prepare(PreparingEnlistment preparingEnlistment);
        void Rollback(Enlistment enlistment);
    }

    /*
        * TransactionalStringManager è una semplice classe che dimostra una possibile
        * implementazione di IEnlistmentNotification per rendere transazionale
        * la variazione del valore della proprietà pubblica Value.
        * Ci sono n modi per ottenere questa funzionalità: in questo caso lo 
        * scopo era quello di spiegare come fosse possibile inserire un proprio
        * oggetto custom'interno dell'infrastruttura gestita da TransactionScope
        * o da una CommittableTransaction
        */
    public class TransactionalStringManager : IEnlistmentNotification
    {
        public readonly String ID;
        private TransactionCompletedEventHandler onTransactionCompletedHandler;
        /*
            * filed privato per tenere traccia dell'eventuale 
            * transazione in cui siamo coinvolti
            */
        private Transaction _enlistedTransaction;
        private Boolean EnlistedInTransaction
        {
            get { return this._enlistedTransaction != null; }
        }

        public TransactionalStringManager(String ID, String value)
        {
            this.ID = ID;
            this._value = value;

            this.onTransactionCompletedHandler = new
            TransactionCompletedEventHandler(
            this.OnEnlistedTransactionCompleted);
        }

        void EnlistInTransaction(String value)
        {
            if (!this.EnlistedInTransaction)
            {
                Console.WriteLine("{0}: EnlistInTransaction", this.ID);

                /*
                    * Dobbiamo inserirci in una Transazione:
                    *-teniamo un riferimento alla transazione in 
                    *cui andiamo ad inserirci
                    *-chiediamo alla transazione di inserirci all'interno 
                    * del suo processo
                    */
                this._enlistedTransaction = Transaction.Current;
                this._enlistedTransaction.EnlistVolatile(
                this,
                EnlistmentOptions.None);

                /*
                    * Ci interessa tenere traccia di quando la transazione finisce
                    * per liberare un po' di risorse
                    */
                this._enlistedTransaction.TransactionCompleted +=
                this.onTransactionCompletedHandler;

                /*
                    * Teniamo traccia del valore in modo da poter 
                    * effettuare un Rollback
                    */
                this.temporaryValue = value;
            }
        }

        /*
            * Il field "_value" ospita il valore della stringa
            * direttamente se non siamo coinvolti in una transazione
            * oppure solo ed esclusivamente al termine della stessa
            * dopo il Commit
            */
        private String _value;

        /*
            * il field "temporaryValue" serve per ospitare il valore
            * della stringa durante la transazione fino al Commit o
            * ad un eventuale Rollback
            */
        private String temporaryValue;

        /*
            * la proprietà pubblica a cui accedere per cambiare
            * il valore memorizzato
            */
        public String Value
        {
            get
            {
                if (!this.EnlistedInTransaction)
                {
                    if (Transaction.Current == null)
                    {
                        /*
                            * se NON siamo in una transazione e NON ce n’è
                            * alcuna attiva ci limitiamo a ritornare il 
                            * valore memorizzato, come farebbe una qualsiasi
                            * classe senza il supporto per le transazioni
                            */
                        return this._value;
                    }
                    else
                    {
                        /*
                            * NON siamo in una transazione ma ce n’è una attiva
                            * chiediamo di essere coinvolti nella transazione
                            */
                        this.EnlistInTransaction(this._value);
                    }
                }
                else if (this._enlistedTransaction != Transaction.Current)
                {
                    /*
                        * Siamo già in una transazione che però è diversa 
                        * da quella corrente. Non è possibile essere coinvolti
                        * in 2 transazioni in contemporanea
                        */
                    throw new InvalidOperationException(
                    "Already Enlisted in a Transaction");
                }

                /*
                    * Se arriviamo qui significa che siamo in una transazione
                    * ritorniamo quindi il valore reale al fine di rispettare il
                    * 3° principo ACID: Isolated, questo ci garantisce che una 
                    * richiesta del valore durante la transazione ritorni ancora
                    * il valore vecchio; se non volessimo rispettare il principio
                    * Isolated (che non è detto sia proprio un male) basterebbe
                    * ritornare il valore di temporaryValue
                    */
                return this._value;
            }
            set
            {
                if (!this.EnlistedInTransaction)
                {
                    if (Transaction.Current == null)
                    {
                        /*
                            * se NON siamo in una transazione e NON ce n’è
                            * alcuna attiva ci limitiamo a settare il 
                            * valore in arrivo, come farebbe una qualsiasi
                            * classe senza il supporto per le transazioni
                            */
                        this._value = value;
                    }
                    else
                    {
                        /*
                            * NON siamo in una transazione ma ce n’è una attiva
                            * chiediamo di essere coinvolti nella transazione
                            */
                        this.EnlistInTransaction(value);
                    }
                }
                else if (this._enlistedTransaction != Transaction.Current)
                {
                    /*
                        * Siamo già in una transazione che però è diversa 
                        * da quella corrente. Non è possibile essere coinvolti
                        * in 2 transazioni in contemporanea
                        */
                    throw new InvalidOperationException(
                    "Already Enlisted in a Transaction");
                }
                else
                {
                    /*
                        * Siamo già nella transazione, impostiamo quindi il valore in
                        * arrivo nella nostra variabile temporanea
                        */
                    this.temporaryValue = value;
                }
            }
        }

        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            /*
                * In fase di commit non facciamo altro che memorizzare
                * definitivamente il valore che avevamo nella variabile
                * temporanea
                */
            this._value = this.temporaryValue;

            /*
                * Informiamo il Transation Manager che abbiamo completato
                * il nostro lavoro
                */
            enlistment.Done();
        }

        // In fase di Commit() ci limitiamo a persistere il nostro valore e a confermare la Commit;
        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            /*
                * InDoubt viene chiamato dal Transaction Manager nel momento
                * in cui, in fase di commit (Single Phase), uno degli attori
                * perde la connessione con lo storage persistente. Non siamo quindi
                * in grado di sapere lo stato della transazione
                * 
                * In questo caso di limitiamo ad accettare lo stato, nel nostro esempio
                * non verrà mai chiamato
                */
            enlistment.Done();
        }

        // Per il tipo di transazioni a cui participiamo InDoubt() non verrà mai chiamato, ci limitiamo quindi a confermare la nostra posizione;
        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            /*
                * In una transazione a 2 fasi il Transaction Manager chiama
                * la Prepare per sapere se tutti i pertecipanti sono pronti
                * ad eseguire il Commit della transazione; la chiamata a 
                * Prepared() indica che siamo pronti ad eseguire la Commit()
                * 
                * Se in questa fase volessimo solo essere degli "spettatori"
                * è possibile chiamare Done() al posto di Prepared() e la Commit()
                * non verrà chiamata!
                * 
                * Se invece vogliamo chiedere che venga forzato un Rollback in questa
                * fase possiamo chiamare ForceRollback().
                */
            preparingEnlistment.Prepared();
        }
        // Anche la fase di Prepare() non comporta alcun lavoro da parte nostra quindi confermiamo che siamo pronti per il Commit;
        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            /*
                * la Rollback viene chiamata in caso di fallimento della
                * transazione; nel nostro caso non facciamo nulla perchè
                * il valore temporaneo è già nella variabile temporanea
                * che al termine della transazione verrà svuotata facendo
                * sì che ad un eventuale accesso post transazione venga 
                * correttamente restituito il valore pre transazione
                */
            enlistment.Done();
        }

        //Infine in caso di Rollback() lasciamo che le cose fluiscano senza intervenire perchè durante la transazione ci siamo sempre appoggiati ad una variabile temporanea che fuori dalla transazione non verrà più usata.
        void OnEnlistedTransactionCompleted(object sender, TransactionEventArgs e)
        {
            /*
                * La transazione è stata completata: qui non ci interessa sapere se
                * con successo o meno, ci limitiamo solo a liberare risorse
                */
            this._enlistedTransaction.TransactionCompleted -=
            this.onTransactionCompletedHandler;
            this._enlistedTransaction = null;
            this.temporaryValue = null;
        }
    }
}

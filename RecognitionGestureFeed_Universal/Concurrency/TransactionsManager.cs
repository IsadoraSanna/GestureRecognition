using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Gestione Transazioni
using System.Transactions;
// Modifies
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;

namespace RecognitionGestureFeed_Universal.Concurrency
{
    /*public interface IEnlistmentNotification
    {
        void Commit(Enlistment enlistment);
        void InDoubt(Enlistment enlistment);
        void Prepare(PreparingEnlistment preparingEnlistment);
        void Rollback(Enlistment enlistment);
    }*/

    /// <summary>
    /// Attualmente avremo a che fare solo con transazioni locali (distribuite in futuro?). Per far ciò abbiamo bisogno
    /// di: 1) Local Transaction Manager (che si occupa di mantenere un log, da utilizzare in caso di errore), e che
    /// coordina le transazioni su singole risorse; 2) Transaction Coordinator, che si occupa invece di far partire
    /// l'esecuzione delle transazioni, e il risultato finale sarà il commit o il rollback delle operazioni. 
    /// 
    /// Classe che si occupa di gestire la concorrenza tra più transazioni. Quando più transazioni operano su oggetti
    /// diversi, queste vengono eseguite senza problemi. Viceversa entra in gioco il concetto di gestione della 
    /// concorrenza, ottenuta utilizzando gli elementi messi a disposizione dalla classe System.Transactions.
    /// Quando una gesture viene eseguita correttamente, la sua funzione viene mandata alla TransactionsManager
    /// che si occuperà di verificare la presenza di elementi in conflitto e della loro esecuzione.
    /// </summary>
    public class TransactionsManager : IEnlistmentNotification
    {
        // Indica se è già in atto una transazione
        private bool isDoTransactions;
        // Numero massimo di transazioni in esecuzione contemporaneamente
        private readonly int numTransactionManageable = 5;
        private int numTransactionInExecution = 0; // Contatore che indica il numero di transazioni in esecuzione
        // Transazione con lo scope
        private Transaction _enlistedTransaction;

        public void OnTransactionExcute()
        {
            /*
            // Se non c'è uno scope attivo, allora lo sia avvia
            if (numTransactionInExecution == 0)
            {
                numTransactionInExecution++;// Inserire codice per atomicità
                startTransaction();
            }
            else if(numTransactionInExecution < numTransactionManageable)
            {
                // Creo una transazione e cerco di inserirla nello scope previsto
            }
            else
            {
                // Attendi 
            }*/

            // Se non ci sono già altre transazioni in esecuzione allora faccio partire lo scope, in modo da poterle 
            // eseguire in sicurezza. Altrimenti creo una transazione e la invio allo scope.
            if(!isDoTransactions)
            {
                isDoTransactions = true;
                startTransaction();
            }
            else
            {    
                // Crea transazione
                _enlistedTransaction = Transaction.Current;
                _enlistedTransaction.EnlistVolatile(this,  EnlistmentOptions.None);
                // Funzione che si occupa di liberare le risorse una volta che la transazione è stata completata
                this._enlistedTransaction.TransactionCompleted += this.onTransactionCompletedHandler;
            }
        }
        
        // Questa funzione fa partire l'esecuzione di una transazione
        private void startTransaction()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                // Esecuzione della transazione

                // Transazione Completata
                scope.Complete();
            }
            isDoTransactions = false;
        }

        // Funzione richiamata quando viene completata l'esecuzione di una transazione
        private void onTransactionCompletedHandler(object sender, TransactionEventArgs e)
        {
            this._enlistedTransaction.TransactionCompleted -= this.onTransactionCompletedHandler;
            this._enlistedTransaction = null;
        }
        
        /* Implementazione dell'interfaccia IEnlistmentNotification */
        // Commit
        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            // In fase di Commit memorizziamo definitivamente il valore della variabile

            // Comunica al Transation Manager che la transazione è stata conclusa
            enlistment.Done();
        }
        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            // Questa funzione viene chiamata quando uno degli attori perde la connessione con lo storage
            // persistente. Non si può mai verificare.
            enlistment.Done();
        }
        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            // Comunica al Transaction Manager che tutti i partecipanti sono pronti
            preparingEnlistment.Prepared();
        }
        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            // Rollback, vuol dire che qualcosa non è andato come previsto, nel nostro
            // caso non facciamo nulla, in quanto i valori non sono stati ancora assegnati.
            enlistment.Done();
        }
    }
}

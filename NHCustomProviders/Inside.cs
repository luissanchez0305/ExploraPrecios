using System;
using System.Diagnostics.CodeAnalysis;
using NHibernate;

namespace NHCustomProviders
{

	#region Delegates

	/// <summary>Delegate for functions.</summary>
	public delegate T Func<T>(ISession session, ITransaction transaction);

	/// <summary>Delegate for procedures.</summary>
	public delegate void Proc(ISession session, ITransaction transaction);

	#endregion

	///<summary>Helper class for using transactions.</summary>
	public static class Inside
	{
		#region Methods

		/// <summary>Executes the specified code inside a transaction.</summary>
		/// <typeparam name="T">The type returned by the code to execute.</typeparam>
		/// <param name="sessionFactory">The session factory to initiate the transaction.</param>
		/// <param name="codeToExecute">The code to execute inside the transaction.</param>
		/// <returns>The return value from the code executed.</returns>
		public static T Transaction<T>(ISessionFactory sessionFactory, Func<T> codeToExecute)
		{
			T result = default(T);

			Transaction(sessionFactory, delegate(ISession session, ITransaction transaction) { result = codeToExecute(session, transaction); });

			return result;
		}

		/// <summary>Executes the specified code inside a transaction.</summary>
		/// <param name="sessionFactory">The session factory to initiate the transaction.</param>
		/// <param name="codeToExecute">The code to execute inside the transaction.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is a fair use as we don't want the exception to propagate")]
		public static void Transaction(ISessionFactory sessionFactory, Proc codeToExecute)
		{
			// initiates a transaction
			using (ISession session = sessionFactory.OpenSession()) {
				using (ITransaction trans = session.BeginTransaction()) {
					try {
						// execute the code inside the transaction
						codeToExecute(session, trans);
					} catch {
						try {
							// discard the changes
							if ((trans != null) && (trans.IsActive)) {
								trans.Rollback();
							}
						} catch {
						}

						throw;
					} finally {
						// save the changes
						if ((trans != null) && (trans.IsActive)) {
							trans.Commit();
						}
					}
				}
			}
		}

		#endregion
	}
}
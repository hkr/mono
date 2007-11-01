//
// OracleTransaction.cs 
//
// Part of the Mono class libraries at
// mcs/class/System.Data.OracleClient/System.Data.OracleClient
//
// Assembly: System.Data.OracleClient.dll
// Namespace: System.Data.OracleClient
//
// Author: Tim Coleman <tim@timcoleman.com>
//
// Copyright (C) Tim Coleman, 2003
//
// Licensed under the MIT/X11 License.
//

using System;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient.Oci;

namespace System.Data.OracleClient {
	public sealed class OracleTransaction :
#if NET_2_0	
	Common.DbTransaction, IDbTransaction, IDisposable
#else
	MarshalByRefObject, IDbTransaction, IDisposable
#endif
	{
		#region Fields

		OracleConnection connection;
		IsolationLevel isolationLevel;
		bool disposed = false;
		OciTransactionHandle transaction;
		bool isOpen;

		#endregion // Fields

		#region Constructors

		internal OracleTransaction (OracleConnection connection, IsolationLevel isolevel, OciTransactionHandle transaction)
		{
			this.connection = connection;
			this.isolationLevel = isolevel;
			this.transaction = transaction;
			isOpen = true;
		}

		#endregion // Constructors

		#region Properties

		internal bool IsOpen {
			get { return isOpen; }
		}

		public OracleConnection Connection {
			get { return connection; }
		}
		
#if NET_2_0
		[MonoTODO]
		protected override Common.DbConnection DbConnection {
			get { return null; }
		}
#endif

		public
#if NET_2_0
		override
#endif
		IsolationLevel IsolationLevel {
			get { return isolationLevel; }
		}

		IDbConnection IDbTransaction.Connection {
			get { return Connection; }
		}

		#endregion // Properties

		#region Methods

		internal void AttachToServiceContext ()
		{
			transaction.AttachToServiceContext ();
		}

		public
#if NET_2_0
		override
#endif
		void Commit ()
		{
			transaction.Commit ();
			Connection.Transaction = null;
			isOpen = false;
		}

		private void Dispose (bool disposing)
		{
			if (!disposed) {
				if (disposing) {
					if (isOpen)
						Rollback ();

					transaction.Dispose();
				}
				disposed = true;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public
#if NET_2_0
		override
#endif
		void Rollback ()
		{
			transaction.Rollback ();
			Connection.Transaction = null;
			isOpen = false;
		}

		#endregion // Methods
	}
}

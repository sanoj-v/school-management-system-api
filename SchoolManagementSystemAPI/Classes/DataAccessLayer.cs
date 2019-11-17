using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
namespace SchoolManagementSystemAPI.Classes
{
    public class DataAccessLayer
    {

        Hashtable paramCache = new Hashtable(new Hashtable());
        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params object[] parameterValues)
        {

            SqlParameter[] commandParameters = null;

            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) & parameterValues.Length > 0)
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)

                commandParameters = GetSpParameterSet(connectionString, commandText, false);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

            }
            //create a command and prepare it for execution
            SqlConnection GstCon = new SqlConnection(connectionString);
            SqlCommand GstCommand = new SqlCommand();
            DataSet GstDS = new DataSet();
            //SqlDataAdapter GstDA = default(SqlDataAdapter);
            int retval = 0;

            GstCommand.CommandTimeout = 2000;

            PrepareCommand(GstCommand, GstCon, (SqlTransaction)null, commandType, commandText, commandParameters);

            //finally, execute the command.
            retval = GstCommand.ExecuteNonQuery();

            //detach the SqlParameters from the command object, so they can be used again
            GstCommand.Parameters.Clear();
            return retval;
        }
        //ExecuteNonQuery

        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params object[] parameterValues)
        {

            SqlParameter[] commandParameters = null;

            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) & parameterValues.Length > 0)
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = GetSpParameterSet(connectionString, commandText, false);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);
            }
            //create a command and prepare it for execution
            SqlConnection GstCon = new SqlConnection(connectionString);
            SqlCommand GstCommand = new SqlCommand();
            DataSet GstDS = new DataSet();
            SqlDataAdapter GstDA = default(SqlDataAdapter);
            try
            {
                GstCommand.CommandTimeout = 2000;

                PrepareCommand(GstCommand, GstCon, (SqlTransaction)null, commandType, commandText, commandParameters);

                //create the DataAdapter & DataSet
                GstDA = new SqlDataAdapter(GstCommand);
                try
                {
                    //fill the DataSet using default values for DataTable names, etc.
                    GstDA.Fill(GstDS);
                }
                finally
                {
                    if ((GstDA != null))
                        GstDA.Dispose();
                    //detach the SqlParameters from the command object, so they can be used again
                    GstCommand.Parameters.Clear();
                }
            }
            finally
            {
                if (GstCon.State == ConnectionState.Open)
                    GstCon.Close();
                //GstDA.Dispose()
            }
            //return the dataset
            return GstDS;
        }
        //ExecuteDataset


        private void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            short i = 0;
            short j = 0;

            if ((commandParameters == null) & (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            short p = 0;
            for (p = 0; p <= parameterValues.Length - 1; p++)
            {
                if ((parameterValues[p] != null))
                {
                    if (parameterValues[p].GetType().Name == "String")
                    {
                        parameterValues[p] = parameterValues[p].ToString().Replace("|;amp;|", "&");
                    }
                }
            }

            //value array
            j = Convert.ToInt16(commandParameters.Length - 1);
            for (i = 0; i <= j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
                //If Not IsNothing(parameterValues(i)) Then
                //    If parameterValues(i).GetType.Name = "String" Then
                //        commandParameters(i).Value = parameterValues(i).ToString.Replace("|;amp;|", "&")
                //    Else
                //        commandParameters(i).Value = parameterValues(i)
                //    End If
                //Else
                //    commandParameters(i).Value = parameterValues(i)
                //End If
            }

        }
        //AssignParameterValues

        public SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {


            SqlParameter[] cachedParameters = null;
            string hashKey = null;

            hashKey = connectionString + ":" + spName + (includeReturnValueParameter == true ? ":include ReturnValue Parameter" : "");

            //  cachedParameters == (SqlParameter[]) paramCache(hashKey);
            cachedParameters = (SqlParameter[])paramCache[hashKey];

            if ((cachedParameters == null))
            {
                paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter);
                cachedParameters = (SqlParameter[])paramCache[hashKey];

            }

            return CloneParameters(cachedParameters);

        }



        //GetSpParameterSet

        //deep copy of cached SqlParameter array
        private SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {

            short i = 0;
            short j = Convert.ToInt16(originalParameters.Length - 1);
            SqlParameter[] clonedParameters = new SqlParameter[j + 1];

            for (i = 0; i <= j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }
        //CloneParameters

        private SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter, params object[] parameterValues)
        {

            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(spName, cn);
            SqlParameter[] discoveredParameters = null;

            try
            {
                cn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 2000;

                SqlCommandBuilder.DeriveParameters(cmd);
                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                discoveredParameters = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(discoveredParameters, 0);
            }
            finally
            {
                cmd.Dispose();
                cn.Dispose();

            }

            return discoveredParameters;

        }
        //DiscoverSpParameterSet


        private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            command.CommandTimeout = 2000;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if ((transaction != null))
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if ((commandParameters != null))
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }
        //PrepareCommand

        private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            //SqlParameter p = default(SqlParameter);

            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if (p.Direction == ParameterDirection.InputOutput & p.Value == null)
                {
                    p.Value = null;
                }
                command.Parameters.Add(p);
            }
        }
        //AttachParameters


        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params object[] parameterValues)
        {
            SqlParameter[] commandParameters = null;

            //if we receive parameter values, we need to figure out where they go

            if ((parameterValues != null) & parameterValues.Length > 0)
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = GetSpParameterSet(connectionString, commandText, false);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);
            }

            //create a command and prepare it for execution
            SqlConnection gstCon = new SqlConnection(connectionString);
            SqlCommand gstCommand = new SqlCommand();
            object retval = null;

            gstCommand.CommandTimeout = 2000;

            PrepareCommand(gstCommand, gstCon, (SqlTransaction)null, commandType, commandText, commandParameters);

            //execute the command & return the results
            retval = gstCommand.ExecuteScalar();

            //detach the SqlParameters from the command object, so they can be used again
            gstCommand.Parameters.Clear();

            return retval;
        }



        public SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params object[] parameterValues)
        {
            SqlParameter[] commandParameters = null;


            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) & parameterValues.Length > 0)
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                commandParameters = GetSpParameterSet(connectionString, commandText, false);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);
            }

            //create a command and prepare it for execution
            SqlConnection gstCon = new SqlConnection(connectionString);
            SqlCommand gstCommand = new SqlCommand();
            SqlDataReader gstReader = default(SqlDataReader);

            gstCommand.CommandTimeout = 2000;

            PrepareCommand(gstCommand, gstCon, (SqlTransaction)null, commandType, commandText, commandParameters);

            //execute the command & return the results
            gstReader = gstCommand.ExecuteReader(CommandBehavior.CloseConnection);

            //detach the SqlParameters from the command object, so they can be used again
            gstCommand.Parameters.Clear();

            return gstReader;

        }
        //ExecuteReader


        // To detect redundant calls
        private bool disposedValue = false;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free other state (managed objects).
                }

                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }



        #region " IDisposable Support "
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion







    }
}


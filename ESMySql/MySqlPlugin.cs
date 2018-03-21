using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Text;
using GHMatti.MySQL;
using GHMatti.Core;

namespace ESMySql
{
    class MySqlPlugin : BaseScript
    {
        bool _mReady = false;
        private MySQL _mySQL;
        private GHMattiTaskScheduler _scheduler;

        public MySqlPlugin()
        {
            _scheduler = new GHMattiTaskScheduler();
            Initialization();
        }

        private void Initialization()
        {
            Utils.ReleaseWrite($"Thank you for choosing Friends in Code. {Environment.NewLine} Enjoy this Essential Mode Mysql Plugin. {Environment.NewLine} Credits to zr0iq for this wonderful async library for Mysql{Environment.NewLine}");
            MySQLSettings settings = new MySQLSettings();
            settings.ConvarConnectionString = API.GetConvar("mysql_connection_string", "");
            settings.ConvarDebug = API.GetConvar("mysql_debug", "true");
            settings.XMLConfiguration = new Dictionary<string, string>() { { "MySQL:UseConvars", "true" } };

            EventHandlers.Add("es_db:firstRunCheck", new Action(() =>
                {
                    Utils.DebugWriteLine("First check is done nothing to see here :P");
                }));

            EventHandlers.Add("es_db:doesUserExist", new Action<string, CallbackDelegate>(async (identifier, cb) =>
            {
                Utils.DebugWriteLine($"Checking for user with {identifier}");
                MySQLResult result = await _mySQL.QueryResult($"SELECT * FROM users WHERE `identifier`=@identifier;", new Dictionary<string, dynamic> { { "@identifier", identifier } });
                await Delay(0);
                cb.Invoke((result.Count == 1));
            }));
            //Check if user exists
            EventHandlers.Add("es_db:retrieveUser", new Action<string, CallbackDelegate>(async (identifier, cb) =>
                {
                    Utils.DebugWriteLine($"Retriving user for {identifier}");
                    MySQLResult result = await _mySQL.QueryResult("SELECT * FROM users WHERE identifier=@identifier;", new Dictionary<string, dynamic> { { "@identifier", identifier } });
                    await Delay(0);
                    if (result.Count >= 1)
                    {

                        cb.Invoke(result[0]);
                    }
                    else
                    {
                        cb.Invoke(false);
                    }
                }));

            //Create new Essentialmode user
            EventHandlers.Add("es_db:createUser", new Action<string, string, int, int>((identifier, license, cash, bank) =>
               {
                   Utils.DebugWriteLine($"Creating user for {identifier}");
                   _mySQL.Query("INSERT INTO users (`identifier`, `money`, `bank`, `group`, `permission_level`, `license`) VALUES (@identifier, @cash, @bank, 'user', 0, @license);", new Dictionary<string, dynamic> {
                       {"@identifier", identifier},
                       {"@license", license},
                       {"@cash", cash},
                       {"@bank", bank},
                   });
               }));

            //Retrieve User by license
            EventHandlers.Add("es_db:retrieveLicensedUser", new Action<string, CallbackDelegate>(async (identifier, cb) =>
            {
                Utils.DebugWriteLine($"Retriving user with license {identifier}");
                MySQLResult result = await _mySQL.QueryResult("SELECT * FROM users WHERE `license`=@license;", new Dictionary<string, dynamic> { { "@license", identifier } });
                await Delay(0);
                if (result.Count >= 1)
                {
                    cb.Invoke(result[0]);
                }
                else
                {
                    cb.Invoke(false);
                }
            }));

            //Licensed Users check
            EventHandlers.Add("es_db:doesLicensedUserExist", new Action<string, CallbackDelegate>(async (identifier, cb) =>
            {
                Utils.DebugWriteLine($"Checking for user with license {identifier}");
                MySQLResult result = await _mySQL.QueryResult("SELECT * FROM users WHERE `license`=@license;", new Dictionary<string, dynamic> { { "@license", identifier } });
                await Delay(0);
                cb.Invoke((result.Count == 1));
            }));

            //Update user
            EventHandlers.Add("es_db:updateUser", new Action<string, dynamic, CallbackDelegate>(async (identifier, update, cb) =>
              {
                  Utils.DebugWriteLine($"Updating user {identifier}");
                  StringBuilder sb = new StringBuilder();
                  IDictionary<string, object> pairs = (IDictionary<string, object>)update;
                  Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                  parameters["@identifier"] = identifier;

                  int updateLength = 0;
                  foreach (KeyValuePair<string, object> kvp in pairs)
                  {
                      parameters["@" + kvp.Key] = kvp.Value;
                      sb.Append(kvp.Key);
                      sb.Append("=@");
                      sb.Append(kvp.Key);
                      updateLength++;
                      if(updateLength != pairs.Count)
                          sb.Append(",");
                  }
                  string query = $"UPDATE users SET {sb.ToString()} WHERE identifier=@identifier;";
                  Utils.DebugWriteLine($"Update query is {query}");
                  long result = await _mySQL.Query(query, parameters);
                  await Delay(0);
                  cb.Invoke((result == 1));
              }));

            _mySQL = new MySQL(settings, _scheduler);
            _mReady = true;

        }
    }
}

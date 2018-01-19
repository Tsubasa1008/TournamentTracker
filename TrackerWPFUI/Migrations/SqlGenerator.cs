using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Migrations
{
    class SqlGenerator : MySql.Data.Entity.MySqlMigrationSqlGenerator
    {
        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            IEnumerable<MigrationStatement> res = base.Generate(migrationOperations, providerManifestToken);

            foreach (MigrationStatement ms in res)
            {
                ms.Sql = ms.Sql.Replace("dbo.", "");

                //if (ms.Sql.Contains("dbo."))
                //{
                //    return new List<MigrationStatement>();
                //}
            }

            return res;
        }
    }
}

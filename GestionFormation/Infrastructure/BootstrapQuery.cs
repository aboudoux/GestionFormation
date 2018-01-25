using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GestionFormation.EventStore;

namespace GestionFormation.Infrastructure
{
    public static class BootstrapQuery
    {        
        public static Task Launch()
        {
            return Task.Run(() =>
            {
                using (var context = new ProjectionContext(ConnectionString.Get()))
                {
                    context.Database.ExecuteSqlCommand("IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CompteurConvention]') AND type = 'SO')\r\n" +
                                                       "BEGIN\r\n" +
                                                       "CREATE SEQUENCE [dbo].[CompteurConvention] AS BIGINT START WITH 6000 INCREMENT BY 1 NO MAXVALUE NO CYCLE CACHE 10\r\n" +
                                                       "END"
                                                       );
                }
            });
        }
    }
}

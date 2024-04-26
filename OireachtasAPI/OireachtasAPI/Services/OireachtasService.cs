using OireachtasAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OireachtasAPI.Services
{
    public class OireachtasService
    {
        private readonly IOireachtasRepository repository;

        public OireachtasService(IOireachtasRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Return bills sponsored by the member with the specified pId
        /// </summary>
        /// <param name="pId">The pId value for the member</param>
        /// <returns>List of bill records</returns>
        public List<dynamic> FilterBillsSponsoredBy(string pId)
        {
            dynamic leg = this.repository.GetLegislationData();
            dynamic mem = this.repository.GetMembersData();

            List<dynamic> ret = new List<dynamic>();

            foreach (dynamic res in leg["results"])
            {
                dynamic p = res["bill"]["sponsors"];
                foreach (dynamic i in p)
                {
                    string name = i["sponsor"]["by"]["showAs"];
                    foreach (dynamic result in mem["results"])
                    {
                        string fname = result["member"]["fullName"];
                        string rpId = result["member"]["pId"];
                        if (fname == name && rpId == pId)
                        {
                            ret.Add(res["bill"]);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Return bills updated within the specified date range
        /// </summary>
        /// <param name="since">The lastUpdated value for the bill should be greater than or equal to this date</param>
        /// <param name="until">The lastUpdated value for the bill should be less than or equal to this date.If unspecified, until will default to today's date</param>
        /// <returns>List of bill records</returns>
        public List<dynamic> FilterBillsByLastUpdated(DateTime since, DateTime until)
        {
            dynamic leg = this.repository.GetLegislationData();

            List<dynamic> ret = new List<dynamic>();

            foreach (dynamic res in leg["results"])
            {
                dynamic p = res["bill"];

                if (p["lastUpdated"] >= since && p["lastUpdated"] <= until)
                {
                    ret.Add(p);
                }
            }
            return ret;
        }
    }
}

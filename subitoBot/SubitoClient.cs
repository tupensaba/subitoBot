using System;
using System.Net;

namespace subitoBot
{
	public class SubitoClient
	{
		public SubitoClient()
		{
		}

        public async Task<string> getHtmlofItems(int page, string sUrl)
        {
            var clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            var client = new HttpClient(clientHandler);
            client.Timeout = TimeSpan.FromSeconds(30);

                sUrl = sUrl.Contains("?") ? (sUrl.Contains("&o=") ? sUrl.Substring(0,sUrl.IndexOf("&o=")) + $"&o={page}" : sUrl + $"&o={page}")
                    :
                    sUrl + $"?o={page}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(sUrl),
                Headers =
                {
                    { "authority", "www.subito.it" },
                    { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
                    { "accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7" },
                    { "cache-control", "max-age=0" },
                    { "cookie", "kppid=A69A3820A54F24B225752CD6; _ga=GA1.1.614435635.1703010530; didomi_token=eyJ1c2VyX2lkIjoiMThjODM1NjYtYjIzMS02MmM0LWIzYWYtMzY2MGQ2ZTZiNzIyIiwiY3JlYXRlZCI6IjIwMjMtMTItMTlUMTg6Mjg6NDguMDUyWiIsInVwZGF0ZWQiOiIyMDIzLTEyLTE5VDE4OjI4OjUwLjE5MVoiLCJ2ZW5kb3JzIjp7ImVuYWJsZWQiOlsiZ29vZ2xlIiwiYzphbXBsaXR1ZGUiLCJjOm1pY3Jvc29mdC1vbmVkcml2ZS1saXZlLXNkayIsImM6YXBwc2ZseWVyLTlqY3duaVk5IiwiYzpiaW5nLWFkcyIsImM6ZGlkb21pIiwiYzprcnV4LWRpZ2l0YWwiLCJjOnF1YW50Y2FzdC1tZWFzdXJlbWVudCIsImM6b21uaXR1cmUtYWRvYmUtYW5hbHl0aWNzIiwiYzpncmFwZXNob3QiLCJjOnF1YW50dW0tYWR2ZXJ0aXNpbmciLCJjOngxIiwiYzp0dWJlbW9ndWwiLCJjOnNlZ21lbnQiLCJjOmFkbW9iIiwiYzptZWRhbGxpYS1paHpyeUZMWSIsImM6c2FsZXNmb3JjZS1DUEJGRWZIUCIsImM6b3B0aW1pemVseS1FTlFFaWlaVCIsImM6YWthY2Rvcm9kLVdBRDdpWHRoIiwiYzptaWNyb3NvZnQtYW5hbHl0aWNzIiwiYzphdGludGVybmUtY1dRS0hlSloiLCJjOnBhbmdsZWRzcC1aQnhMaGdDVyIsImM6Ymx1ZWthaSIsImM6Z29vZ2xlYW5hLTRUWG5KaWdSIiwiYzpzb2Npb21hbnRpLW1NVEc4eGc0IiwiYzphd3MtY2xvdWRmcm9udCIsImM6c3VibGltZXNrLXBaN0FueTdHIiwiYzptZWV0cmljc2ctTmRFTjhXeFQiLCJjOmNlbnRyby1pVVdWbU40TiIsImM6bWljcm9zb2Z0IiwiYzpwaW50ZXJlc3QiXX0sInB1cnBvc2VzIjp7ImVuYWJsZWQiOlsiY29va2llYW5hLWtDQ1JHejRtIiwiY29va2llZGktbk5lMjNRamQiLCJjb29raWV0ZWMtOXFmQUVWd2siLCJhdWRpZW5jZW0teGVkZVUyZ1EiLCJnZW9sb2NhdGlvbl9kYXRhIl19LCJ2ZW5kb3JzX2xpIjp7ImVuYWJsZWQiOlsiZ29vZ2xlIiwiYzpwYW5nbGVkc3AtWkJ4TGhnQ1ciLCJjOnNvY2lvbWFudGktbU1URzh4ZzQiLCJjOmNlbnRyby1pVVdWbU40TiJdfSwidmVyc2lvbiI6MiwiYWMiOiJEQmVBV0FGa0JKWUVvZ0pZZ1VWQkxHQ2VjRkNJTFJ3WG9nd1guREJlQVdBRmtCSllFb2dKWWdVVkJMR0NlY0ZDSUxSd1hvZ3dYIn0=; euconsent-v2=CP3CKIAP3CKIAAHABBENAeEoAP_gAELgABCYI3kBxCxUACNAAGBAAMMQAQUGQFAAAkAAAAIBAQABAIIAIAQEgEAAIAAAAAAAAAIAICYAAAAgAAAAAQAAAAAAIAEAAAAQBAAAIAAAAAABAAAAAAAAAIAAAAAAgEAAAAAAgAAAAAIACEAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAIACgAAAAAAAAAAAAAABAII3wIYAFAAWABUAC4AHAAPAAggBiAGQANAAiABMgC2ALgAYgAzAB6AD8AIQARwAygB-gEIAIsARwAuoBfQD2gJiAU2AvMBggDJwGWAP3AjeACQgAwABEGoAAA.f_wACFwAAAAA; _gcl_au=1.1.1468350240.1703010530; displayCookieConsent=y; FPID=FPID2.2.UdwqX2Hw1oE3S1utaBj7I8Nmd8ep91UDYU9BPWaH%2F%2BM%3D.1703010530; FPAU=1.1.1468350240.1703010530; __gads=ID=e2824e70d7c28611:T=1703010530:RT=1703010530:S=ALNI_MZZH5trVMbayiLBhaWm2SNcw7c3EQ; __gpi=UID=00000d21c3463c47:T=1703010530:RT=1703010530:S=ALNI_Mb8tD3rb_HlHR07yIQlZXjN5XBw-A; __rtbh.lid=%7B%22eventType%22%3A%22lid%22%2C%22id%22%3A%22Tte32XEDXxZvbbg99UVn%22%7D; crto_is_user_optout=false; crto_mapped_user_id=oLthvJ8K0Pe7VJ47GueZm-MMTvpGSJ4U; _fbp=fb.1.1703010531426.1511473246; FPLC=%2BuBzj9%2BMQ%2F62UTx12Xg%2BHcx0%2BoZ618c7982xG7AQcn1SF%2BKORbMvf4Co59NszPpOu%2F4tRXihEbEN8ZHpnOPpmhVcQPpbOvWFPFpWJq71gkcr8%2Bmypr0YEV3shf4QWA%3D%3D; akacd_orodha=2177452799~rv=39~id=86a724d27e295a325245d15a5f796fdb; __rtbh.uid=%7B%22eventType%22%3A%22uid%22%7D; _pbjs_userid_consent_data=8708018901051321; _pubcid=d8a80454-88ba-4f94-bed8-7dbfa7c9fcb0; _pin_unauth=dWlkPVl6VmpZVEk1T1RBdE9XVmtZUzAwWmpZeExXSTVOekF0T0RVMk5tUmhabUl4WlRsag; __gsas=ID=40c6575cacac7a6c:T=1703010541:RT=1703010541:S=ALNI_MZzX6cHfQh9wiq0QyQlGgvSvzXJ3A; mdLogger=false; kampyle_userid=a200-428a-f3f4-b69b-5e66-31ff-11fa-c6a4; mdigital_alternative_uuid=8a4f-4f54-b359-8676-e965-9196-bf82-d3f3; kampyleUserSession=1703010547401; kampyleUserSessionsCount=1; kampyleUserPercentile=99.24331394914925; _pulse2data=25870bdb-9d05-4c2b-a443-ad787a55bcb8%2Cv%2C%2C1703011453934%2CeyJpc3N1ZWRBdCI6IjIwMjMtMTItMTlUMTg6Mjg6NDlaIiwiZW5jIjoiQTEyOENCQy1IUzI1NiIsImFsZyI6ImRpciIsImtpZCI6IjIifQ..kmZQXh6I9NBtSNxFFRQiXw.kqb56YswQiAk0h8hPzy-7r9rSgsfDbXIzl5XkQlB3Tg9A55OHMzUEghAym8-4Xd5PSOXDiUcVF80nHnwGBW2YWg80W6Fa1uMmfFE3W8nfjc_3OJ6ZU6BeV4fSek_4Tt0ZmgTn42XCfDMpC58JEza-Hp7JPLmKpzSZeTt-el9uf9DnJw9PtDzTGn4aHXvcUTo8tu_OWB43Fr_Kt2wQ9jFcH-LNSUHb77Q5TTh6Nl90rQ.rxR2yVdFxNlFVZKu_5K-Sw%2C937361454986847383%2C%2Ctrue%2C%2CeyJraWQiOiIyIiwiYWxnIjoiSFMyNTYifQ..eGMh5BFYH4tlAb5BtHP6wxFMVokTPrBMjpPOyYTvqyI; tmSID=1a3dabe1-b8c1-4cc7-8d51-ef57a8059755; _ga_D72SGH4DYJ=GS1.1.1703010529.1.1.1703010659.0.0.0; kampyleSessionPageCounter=6; cto_bundle=yWMlV19UZ3l5cW1OTVYyczNsOUtyaFJaMnUxTWsxbVZNaFlBMzJHcnFXdHlvQWFZV0ElMkZuTjFwdmRjSUhsViUyRjhVJTJGaGVLVnlRSEdCM3F4VlRYRXFYVnI5ZjhsaGZrUjQ1R2RDVHVuRk1HJTJCa0ZqJTJCODRFWnFWTks1ZmJBV3RKdmpuRU83Z1FnckJET3BqMjhqbXFhSHZlUzFxV1h3JTNEJTNE" },
                    { "device-memory", "8" },
                    { "ect", "4g" },
                    { "referer", $"{sUrl}" },
                    //{ "sec-fetch-dest", "document" },
                    //{ "sec-fetch-mode", "navigate" },
                    //{ "sec-fetch-site", "same-origin" },
                    //{ "sec-fetch-user", "?1" },
                    //{ "upgrade-insecure-requests", "1" },
                    { "user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36" },
                },
            };
            try
            {
                using (var response = await client.SendAsync(request))
            {
                
                    //response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    return body;
               
            }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Bad request");
                return "";
            }
        }

	}
}


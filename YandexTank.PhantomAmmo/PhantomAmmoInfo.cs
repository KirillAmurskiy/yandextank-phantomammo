using System.Collections.Generic;
using System.Linq;

namespace YandexTank.PhantomAmmo
{
    public class PhantomAmmoInfo
    {
        public PhantomAmmoInfo(string url, string method)
        {
            Url = url;
            Method = method;
        }

        public static PhantomAmmoInfo MakeGet(string url)
        {
            return new PhantomAmmoInfo(url, "GET");
        }
        
        public string Method { get; }
        
        public string Url { get; } 
        
        public string Body { get; set; }

        public string Status { get; set; } = "good";

        public string Protocol { get; set; } = "HTTP/1.0";
        
        public Dictionary<string,string> Headers { get; } = new Dictionary<string, string>();
        
        public override string ToString()
        {
            var result = $"{Method} {Url} {Protocol}\r\n";
            
            if (Headers.Count > 0)
            {
                result += string.Join("\r\n", Headers.Select(h => $"{h.Key}: {h.Value}")) + "\r\n";
            }
            else
            {
                result += "User-Agent: xxx (shell 1)\r\n";    
            }
            
            if (!string.IsNullOrEmpty(Body))
            {
                if (!Headers.ContainsKey("Content-Length"))
                {
                    result += $"Content-Length: {Body.Length}\r\n";    
                }

                result += $"\r\n{Body}";
            }
            
            result += "\r\n";
            
            var len = result.Length;
            return $"{len} {Status}\n{result}";
        }
    }
}
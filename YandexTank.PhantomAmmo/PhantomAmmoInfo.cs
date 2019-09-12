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
        
        public string[] Headers { get; set; }
        
        public override string ToString()
        {
            var result = $"{Method} {Url} {Protocol}\r\n";
            
            if (Headers != null
                && Headers.Length > 0)
            {
                result += string.Join("\r\n", Headers) + "\r\n";
            }
            else
            {
                result += "User-Agent: xxx (shell 1)\r\n";    
            }
            
            if (!string.IsNullOrEmpty(Body))
            {
                result += $"Content-Length: {Body.Length}\r\n" +
                          "\r\n" +
                          $"{Body}";
            }
            result += "\r\n";
            
            var len = result.Length;
            return $"{len} {Status}\n{result}";
        }
    }
}
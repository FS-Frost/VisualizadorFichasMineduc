using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MineducRbd {
    public static class Utils {
        public static string GetHttpResponse(string url) {
            try {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) {
                    var responseStream = response.GetResponseStream();
                    StreamReader streamReader = null;

                    if (response.CharacterSet == null) {
                        streamReader = new StreamReader(responseStream);
                    }
                    else {
                        streamReader = new StreamReader(
                            responseStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    var data = streamReader.ReadToEnd();

                    response.Close();
                    streamReader.Close();

                    return data;
                }
            }
            catch (Exception) { }

            return null;
        }

        public static string FormatData(string data) {
            const string pattern = @"^\s+(.*)\s+$";
            var regex = new Regex(pattern);
            var match = regex.Match(data);


            if (match.Success) {
                var result = match.Groups[1].Value;
                result = result.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                return result;
            }

            return data;
        }
    }
}

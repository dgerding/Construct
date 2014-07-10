using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConstructGameMediaRecorder
{
    public static class DateTimeUtility
    {
        public static DateTime ConvertToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
            {
                return dateTime;
            }
            return dateTime.ToUniversalTime();
        }

        public static DateTime ConvertToLocalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
            {
                return dateTime;
            }
            return dateTime.ToLocalTime();
        }

        public static string FormatTimeAndDurationAsIso8601(DateTime start, double durationInMilliSeconds)
        {
            StringBuilder sb = new StringBuilder();
            string dateTimeToken = FormatDateTimeAsIso8601(start, true);
            sb.AppendFormat("{0}_{1}", dateTimeToken, durationInMilliSeconds);
            return sb.ToString();
        }

        public static string FormatDateTimeAsIso8601(DateTime time, bool includeMilliseconds)
        {
            StringBuilder sb = new StringBuilder();
            string dateText = time.ToString("yyyy-MM-dd");
            string timeText = includeMilliseconds ? time.ToString("HH-mm-ss-fff") : time.ToString("HH-mm-ss");
            sb.AppendFormat("{0}T{1}Z", dateText, timeText);
            return sb.ToString();
        }

        public static DateTime ParseDateTimeFromIso8601Format(string text)
        {
            // note the letters T and Z in the formats below. T demarcates the date from the time and Z just means zulu (UTC)
            //
            // http://en.wikipedia.org/wiki/ISO_8601#UTC
            //
            // "yyyy-MM-ddTHH:mm:ss.fffZ" (with milliseconds)
            // "yyyy-MM-ddTHH:mm:ss.fffZ" (without milliseconds)

            try
            {
                Match match = Regex.Match(text, @"(?<yyyy>\d\d\d\d)-(?<MM>\d\d)-(?<dd>\d\d)T(?<HH>\d\d)-(?<mm>\d\d)-(?<ss>\d\d)(-(?<fff>\d\d\d))*Z");
                if (match.Success)
                {
                    int month = int.Parse(match.Result("${MM}"));
                    int day = int.Parse(match.Result("${dd}"));
                    int year = int.Parse(match.Result("${yyyy}"));
                    int hour = int.Parse(match.Result("${HH}"));
                    int minute = int.Parse(match.Result("${mm}"));
                    int seconds = int.Parse(match.Result("${ss}"));
                    var millSecondmatch = match.Result("${fff}");
                    int milliseconds = 0;
                    if (string.IsNullOrEmpty(millSecondmatch) == false)
                    {
                        milliseconds = int.Parse(millSecondmatch);
                    }
                    DateTime dateTime = new DateTime(year, month, day, hour, minute, seconds, milliseconds, DateTimeKind.Utc);
                    return dateTime;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

    }
}

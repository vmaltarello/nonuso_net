using Newtonsoft.Json;

namespace Nonuso.Common
{
    public static class ConversionExtensions
    {
        public static TOutput To<TOutput>(this object obj, JsonSerializerSettings? settings = null)
        {
            settings ??= new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject<TOutput>(JsonConvert.SerializeObject(obj), settings)!;
        }

        public static void PopulateWith(this object objTarget, object objSource, JsonSerializerSettings? settings = null)
        {
            JsonConvert.PopulateObject(JsonConvert.SerializeObject(objSource), objTarget, settings);
        }
    }
}

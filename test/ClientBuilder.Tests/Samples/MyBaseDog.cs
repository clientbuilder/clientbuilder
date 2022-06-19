namespace ClientBuilder.Tests.Samples;

public class MyBaseDog<TPreference, TDate> : Dog
{
    public TPreference Preference { get; set; }

    public TDate Birthday { get; set; }
}
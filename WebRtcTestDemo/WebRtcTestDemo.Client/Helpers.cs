namespace WebRtcTestDemo.Client;

public static class Helpers
{
    public static byte[] LoadTestBuffer()
    {
        return File.ReadAllBytes("Data/blue.h264");
    }
}
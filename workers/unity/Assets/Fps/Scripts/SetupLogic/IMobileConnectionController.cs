namespace Fps
{
    public interface IMobileConnectionController
    {
        string IpAddress { get; set; }

        ConnectionScreenController ConnectionScreenController { get; set; }

        void TryConnect();
    }
}

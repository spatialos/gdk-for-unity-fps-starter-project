namespace Improbable.Gdk.Guns
{
    public interface IRecoil
    {
        void Recoil(bool aiming);

        void SetRecoilSettings(GunSettings.RecoilSettings hipRecoilSettings,
            GunSettings.RecoilSettings aimRecoilSettings);
    }
}

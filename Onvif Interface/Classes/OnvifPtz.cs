using Onvif_Interface.OnvifPtzServiceReference;
using System;
using System.Runtime.Serialization;

namespace SDS.Video.Onvif
{
    class OnvifPtz
    {
        private System.Net.IPAddress IP;
        private int Port;
        private string User;
        private string Password;
        private PTZClient PtzClient;
        private Onvif_Interface.OnvifMediaServiceReference.MediaClient MediaClient;

        public OnvifPtz(string ip, int port)
        {
            System.Net.IPAddress.TryParse(ip, out IP);
            Port = port;
            PtzClient = OnvifServices.GetOnvifPTZClient(IP.ToString(), port);
            MediaClient = OnvifServices.GetOnvifMediaClient(IP.ToString(), Port);
        }

        public OnvifPtz(string ip, int port, string user, string password)
        {
            System.Net.IPAddress.TryParse(ip, out IP);
            Port = port;
            User = user;
            Password = password;
            throw new NotImplementedException();
        }

        public void Pan(float speed, string profileToken = "0")
        {
            Onvif_Interface.OnvifMediaServiceReference.Profile mediaProfile = MediaClient.GetProfile(profileToken);

            PTZConfigurationOptions ptzConfigurationOptions = PtzClient.GetConfigurationOptions(mediaProfile.PTZConfiguration.token);

            PTZSpeed velocity = new PTZSpeed();
            velocity.PanTilt = new Vector2D() { x = speed * ptzConfigurationOptions.Spaces.ContinuousPanTiltVelocitySpace[0].XRange.Max, y = 0 };

            PtzClient.ContinuousMove(profileToken, velocity, null);
        }

        public void Tilt(float speed, string profileToken = "0")
        {
            Onvif_Interface.OnvifMediaServiceReference.Profile mediaProfile = MediaClient.GetProfile(profileToken);
            PTZConfigurationOptions ptzConfigurationOptions = PtzClient.GetConfigurationOptions(mediaProfile.PTZConfiguration.token);

            PTZSpeed velocity = new PTZSpeed();
            velocity.PanTilt = new Vector2D() { x = 0, y = speed * ptzConfigurationOptions.Spaces.ContinuousPanTiltVelocitySpace[0].YRange.Max };

            PtzClient.ContinuousMove(profileToken, velocity, null);
        }

        public void Zoom(float speed, string profileToken = "0")
        {
            Onvif_Interface.OnvifMediaServiceReference.Profile mediaProfile = MediaClient.GetProfile(profileToken);
            PTZConfigurationOptions ptzConfigurationOptions = PtzClient.GetConfigurationOptions(mediaProfile.PTZConfiguration.token);

            PTZSpeed velocity = new PTZSpeed();
            velocity.Zoom = new Vector1D() { x = speed * ptzConfigurationOptions.Spaces.ContinuousZoomVelocitySpace[0].XRange.Max };

            PtzClient.ContinuousMove(profileToken, velocity, null);
        }

        public void Stop(string profileToken = "0")
        {
            PtzClient.Stop(profileToken, true, true);
        }

        public void ShowPreset(string profileToken, string presetToken)
        {
            if (IsValidPresetToken(profileToken, presetToken))
            {
                PTZSpeed velocity = new PTZSpeed();
                velocity.PanTilt = new Vector2D() { x = (float)-0.5, y = 0 }; ;

                PtzClient.GotoPreset(profileToken, presetToken, velocity);
            }
            else
            {
                throw new Exception(string.Format("Invalid Preset requsted - preset token {0}", presetToken));
            }
        }

        public bool IsValidPresetToken(string profileToken, string presetToken)
        {
            PTZPreset[] presets = PtzClient.GetPresets(profileToken);
            foreach (PTZPreset p in presets)
            {
                if (p.token == presetToken)
                    return true;
            }

            return false;
        }

        public PTZStatus GetPtzLocation(string profileToken = "0")
        {
            PTZStatus status = PtzClient.GetStatus(profileToken);
            return status;
        }
    }
}

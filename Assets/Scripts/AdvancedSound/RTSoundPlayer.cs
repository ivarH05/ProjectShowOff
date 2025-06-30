using UnityEngine;

namespace AdvancedSound
{
    public class RTSoundPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip clip1;
        [SerializeField] AudioClip clip2;
        [Range(0, 1), SerializeField] float _loudness = .3f;
        [SerializeField] int _channelsReceived;
        int _pos;

        private void Awake()
        {
            if(AudioSettings.speakerMode != AudioSpeakerMode.Stereo)
            {
                AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            _channelsReceived = channels;
            for (int i = 0; i < data.Length; i ++)
            {
                int dif = _pos - ((_pos/channels)*channels);
                float sample = dif == 0 ? clip1.;
                data[i] = Mathf.Sin(_pos * (.03f + .02f * dif)) * (.3f + .3f*dif) * _loudness;
                _pos++;
            }
        }

        struct LoadedMonoAudio
        {
            public AudioClip Clip;
            public float[] Data;
            public int PositionStart;

            public float SampleDataMono(int position)
            {
                position *= Clip.channels;
            }
            public void LoadDataLooping(int position)
            {
                int clipLength = Clip.samples;
                int pos = position % clipLength;
                Clip.GetData(Data, pos);
                PositionStart = pos;
            }
        }
    }
}

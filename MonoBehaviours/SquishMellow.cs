using Unity.Netcode;
using UnityEngine;

namespace SquishCompany.MonoBehaviours
{
    public class SquishMellow : CustomPhysicsProp
    {
        //public Animator triggerAnimator;
        //public Animator danceAnimator;

        public AudioSource noiseAudio;
        public AudioSource noiseAudioFar;
        public AudioSource musicAudio;
        public AudioSource musicAudioFar;

        public AudioClip[] noiseSFX;
        public AudioClip[] noiseSFXFar;

        //public float noiseRange;
        //public float maxLoudness;
        //public float minLoudness;
        //public float minPitch;
        //public float maxPitch;

        private System.Random noisemakerRandom;

        // The interval of sound broadcasts (alerts dogs, ect)
        private float noiseInterval = 1f;

        public NetworkVariable<bool> isPlayingMusic = new NetworkVariable<bool>(
            true, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner
        );
                

        public override void Start()
        {
            base.Start();
            noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);

            noiseAudio = transform.Find("Noise Near").GetComponent<AudioSource>();
            musicAudio = transform.Find("Music Near").GetComponent<AudioSource>();
            noiseAudioFar = noiseAudio.transform.Find("Noise Far").GetComponent<AudioSource>();
            musicAudioFar = musicAudio.transform.Find("Music Far").GetComponent<AudioSource>();

            noiseSFX = null;
            noiseSFXFar = null;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
                isPlayingMusic.Value = true;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (GameNetworkManager.Instance.localPlayerController == null) return;
            

            //int num = noisemakerRandom.Next(0, noiseSFX.Length);
            ////float num2 = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
            ////float pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;

            //if (noiseAudio != null)
            //{
            //    //noiseAudio.pitch = pitch;
            //    noiseAudio?.PlayOneShot(noiseSFX[num]);//, num2);
            //}

            //if (noiseAudioFar != null)
            //{
            //    //noiseAudioFar.pitch = pitch;
            //    noiseAudioFar?.PlayOneShot(noiseSFXFar[num]);//, num2);
            //}

            //WalkieTalkie.TransmitOneShotAudio(noiseAudio, noiseSFX[num]);//, num2);
            ////RoundManager.Instance.PlayAudibleNoise(transform.position, 13, 0.5f, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            //BroadcastAudioSource(noiseAudio, isInShipRoom);
        }


        public override void DiscardItem()
        {
            if (playerHeldBy != null)
                playerHeldBy.equippedUsableItemQE = false;
            
            isBeingUsed = false;
            base.DiscardItem();
        }

        public override void EquipItem()
        {
            base.EquipItem();
            playerHeldBy.equippedUsableItemQE = true;
        }

        public override void ItemInteractLeftRight(bool right)
        {
            base.ItemInteractLeftRight(right);

            Debug.Log("ItemInteractLeftRight");
            if (right) return;
            if (!IsOwner) return;
            Debug.Log("Running");

            // toggle music
            isPlayingMusic.Value = !isPlayingMusic.Value;
        }

        public override void InteractItem()
        {
            base.InteractItem();

        }

        //int timesPlayedWithoutTurningOff = 0;
        public override void Update()
        {
            base.Update();

            if (isPlayingMusic.Value)
            {
                if (!musicAudio.isPlaying)
                {
                    //musicAudio.Play();
                    //musicAudioFar?.Play();
                }

                //if (!isHeld)
                //{
                //    danceAnimator.Play("Dance");
                //}
                //else
                //{
                //    danceAnimator.Play("Idle");
                //}

                if (noiseInterval <= 0f)
                {
                    noiseInterval = 3f;
                    //timesPlayedWithoutTurningOff++;
                    BroadcastAudioSource(noiseAudio, isInShipRoom);
                }
                else
                {
                    noiseInterval -= Time.deltaTime;
                }
            }
            else
            {
                //timesPlayedWithoutTurningOff = 0;
                //danceAnimator.Play("Idle");
                if (musicAudio.isPlaying)
                {
                    //musicAudio.Pause();
                    //musicAudioFar?.Pause();
                }
            }

        }

        public override void DEBUG_PRINT_STATE()
        {
            base.DEBUG_PRINT_STATE();
            Debug.Log("");
            Debug.Log("#### ############ ####");
            Debug.Log("#### SquishMellow ####");

            Debug.Log($"- noiseAudio {noiseAudio}");
            Debug.Log($"- noiseAudioFar {noiseAudioFar}");

            Debug.Log($"- musicAudio {musicAudio}");
            Debug.Log($"- musicAudioFar {musicAudioFar}");

            Debug.Log($"- noiseSFX {noiseSFX}");
            Debug.Log($"- noiseSFXFar {noiseSFXFar}");

            Debug.Log($"- noisemakerRandom {noisemakerRandom}");
            Debug.Log($"- noiseInterval {noiseInterval}");

            Debug.Log($"- isPlayingMusic {isPlayingMusic}");
            Debug.Log("");
        }
    }
}

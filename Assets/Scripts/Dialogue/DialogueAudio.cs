using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAudio : MonoBehaviour
{
    private AudioSource m_typingAudioSource;

    [Tooltip("Characters always sound the same way or sound completely random.")]
    [SerializeField] private bool m_MakePredictable;

    [Header("ScriptableObject")]
    [SerializeField] private DialogueAudioInfoSO m_DefaultAudioInfo;

    [SerializeField] private DialogueAudioInfoSO[] m_AudioInfos;
    private Dictionary<string, DialogueAudioInfoSO> m_AudioInfoDiccionary;

    //Audio configuration
    private AudioClip[] m_dialogueTypingAudioClips;
    private int m_FrequencyLevel;
    private float m_MaxPitch;
    private float m_MinPitch;
    private bool m_avoidOverlap;

    void Awake()
    {
        m_typingAudioSource = this.gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        InitializeAudioInfoDiccionary();
    }

    private void InitializeAudioInfoDiccionary()
    {
        m_AudioInfoDiccionary = new Dictionary<string, DialogueAudioInfoSO>();
        m_AudioInfoDiccionary.Add(m_DefaultAudioInfo.id, m_DefaultAudioInfo);

        foreach (DialogueAudioInfoSO audioInfo in m_AudioInfos)
            m_AudioInfoDiccionary.Add(audioInfo.id, audioInfo);
    }

    //Active hash or make audio random
    public void MakeAudioPredictable(string toggle)
    {
        if (toggle == "enable")
            m_MakePredictable = true;
        else if (toggle == "disable")
            m_MakePredictable = false;
        else
            Debug.LogError("Error on AudioPredict tag: " + toggle + " option not expected");
    }

    //Set all the audio configurations to the chosen charater audio info
    public void SetAudioInfo(string id)
    {
        DialogueAudioInfoSO currentAudioInfo = null;
        m_AudioInfoDiccionary.TryGetValue(id, out currentAudioInfo);
        if (currentAudioInfo != null)
        {
            m_dialogueTypingAudioClips = currentAudioInfo.dialogueTypingAudioClips;
            m_FrequencyLevel = currentAudioInfo.frequencyLevel;
            m_MaxPitch = currentAudioInfo.maxPitch;
            m_MinPitch = currentAudioInfo.minPitch;
            m_avoidOverlap = currentAudioInfo.avoidOverlap;
        }
        else
        {
            Debug.LogError("The audio info not found with the id: " + id);
            m_dialogueTypingAudioClips = m_DefaultAudioInfo.dialogueTypingAudioClips;
            m_FrequencyLevel = m_DefaultAudioInfo.frequencyLevel;
            m_MaxPitch = m_DefaultAudioInfo.maxPitch;
            m_MinPitch = m_DefaultAudioInfo.minPitch;
            m_avoidOverlap = m_DefaultAudioInfo.avoidOverlap;
        }
    }

    public void PlayDialogueSound(int characterDisplayCounter, char currentCharacter)
    {
        if (characterDisplayCounter % m_FrequencyLevel == 0)
        {
            if (m_avoidOverlap)
                m_typingAudioSource.Stop();

            if (m_MakePredictable)
            {
                //Characters always sound the same way or sound completely random.
                int hashCode = currentCharacter.GetHashCode();

                //sound clip
                int clipIndex = hashCode % m_dialogueTypingAudioClips.Length;

                //chose pitch
                int minPitch = (int)(m_MinPitch * 1000);
                int maxPitch = (int)(m_MaxPitch * 1000);
                int pitchRange = maxPitch - minPitch;
                if (pitchRange == 0)
                    m_typingAudioSource.pitch = minPitch;
                else
                {
                    int hashPitchInt = (hashCode % pitchRange) + minPitch;
                    float hasPitch = hashPitchInt / 1000f;
                    m_typingAudioSource.pitch = hasPitch;
                }

                //play audio clip
                m_typingAudioSource.PlayOneShot(m_dialogueTypingAudioClips[clipIndex]);

            }
            else
            {
                //sound clip
                int clipIndex = UnityEngine.Random.Range(0, m_dialogueTypingAudioClips.Length);
                //pitch
                m_typingAudioSource.pitch = UnityEngine.Random.Range(m_MinPitch, m_MaxPitch);
                //play audio clip
                m_typingAudioSource.PlayOneShot(m_dialogueTypingAudioClips[clipIndex]);
            }
        }
    }
}

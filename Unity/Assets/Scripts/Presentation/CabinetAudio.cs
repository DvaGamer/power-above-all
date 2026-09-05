using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    /// <summary>
    /// Original, temporary synthesized foley. These are procedural sketches,
    /// not historical field recordings or a finished professional sound library.
    /// Does not touch gameplay state or UnityEngine.Random.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CabinetAudio : MonoBehaviour
    {
        private const int SampleRate = 22050;
        private const int VoiceCount = 3;
        private static readonly string[] CueNames = {
            "paper", "quill", "seal", "order", "march", "week", "volley", "hit", "victory", "defeat"
        };
        private static readonly float[] Durations = { .30f, .22f, .18f, .16f, .42f, .48f, .44f, .12f, .38f, .48f };
        private static readonly float[] Gains = { .30f, .28f, .36f, .32f, .36f, .38f, .42f, .25f, .40f, .38f };
        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>(StringComparer.Ordinal);
        private readonly Dictionary<string, float> lastCue = new Dictionary<string, float>(StringComparer.Ordinal);
        private readonly AudioSource[] voices = new AudioSource[VoiceCount];
        private readonly GameObject[] voiceObjects = new GameObject[VoiceCount];
        private float lastAny = -100f;
        private bool ready;
        public bool Muted { get; private set; }

        private void Awake() { Prepare(); }

        private void Prepare()
        {
            if (ready) return;
            for (int i = 0; i < VoiceCount; i++)
            {
                var child = new GameObject("Cabinet Foley Voice " + i);
                child.transform.SetParent(transform, false);
                var source = child.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;
                source.volume = .40f;
                source.priority = 160;
                source.mute = Muted;
                voices[i] = source;
                voiceObjects[i] = child;
            }
            for (int i = 0; i < CueNames.Length; i++) clips.Add(CueNames[i], Generate(i));
            ready = true;
        }

        /// <summary>Unknown or throttled cues are ignored. Maximum three simultaneous voices.</summary>
        public void Play(string cue)
        {
            if (Muted || !isActiveAndEnabled || string.IsNullOrEmpty(cue)) return;
            Prepare();
            cue = cue.Trim().ToLowerInvariant();
            // Mevcut taslaklardan ayrı ağırlıklar; yeni bir ses sistemi değildir.
            switch(cue)
            {
                case "bread": cue="paper";break;
                case "tax": cue="quill";break;
                case "recruit": cue="order";break;
                case "subsidy": cue="seal";break;
            }
            AudioClip clip;
            if (!clips.TryGetValue(cue, out clip)) return;
            float now = Time.unscaledTime;
            if (now - lastAny < .045f) return;
            float previous;
            float cooldown = cue == "volley" ? .20f : cue == "hit" ? .12f :
                cue == "victory" || cue == "defeat" ? 1f : .09f;
            if (lastCue.TryGetValue(cue, out previous) && now - previous < cooldown) return;
            AudioSource voice = null;
            for (int i = 0; i < voices.Length; i++)
                if (voices[i] != null && !voices[i].isPlaying) { voice = voices[i]; break; }
            // A busy pool drops the sound rather than stacking or truncating clips.
            if (voice == null) return;
            voice.clip = clip;
            voice.volume=Gains[Array.IndexOf(CueNames,cue)];
            voice.Play();
            lastAny = now;
            lastCue[cue] = now;
        }

        public void SetMuted(bool muted)
        {
            Muted = muted;
            foreach (var voice in voices)
            {
                if (voice == null) continue;
                voice.mute = muted;
                if (muted) voice.Stop();
            }
        }

        private void OnDisable()
        {
            foreach (var voice in voices) if (voice != null) voice.Stop();
            lastAny = -100f;
            lastCue.Clear();
        }

        private void OnDestroy()
        {
            foreach (var voice in voices) if (voice != null) voice.Stop();
            foreach (var clip in clips.Values) Release(clip);
            clips.Clear();
            foreach (var child in voiceObjects) Release(child);
            ready = false;
        }

        private static void Release(UnityEngine.Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Destroy(value); else DestroyImmediate(value);
        }

        private static float Pulse(float t, float start, float decay)
        {
            if (t < start) return 0f;
            float age = t - start;
            return (1f - Mathf.Exp(-age * 1700f)) * Mathf.Exp(-age * decay);
        }

        private static AudioClip Generate(int kind)
        {
            float length = Durations[kind];
            int count = Mathf.CeilToInt(length * SampleRate);
            var samples = new float[count];
            var noise = new System.Random(1789 + kind * 617);
            float low = 0f, mid = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / SampleRate;
                float white = (float)(noise.NextDouble() * 2d - 1d);
                low += .055f * (white - low);
                mid += .32f * (white - mid);
                float high = white - mid;
                float body = Mathf.Sin(t * 2f * Mathf.PI * (kind == 8 ? 175f : 105f));
                float signal;
                switch (kind)
                {
                    case 0: // Turning paper: two uneven, soft broadband folds.
                        signal = (high * .27f + mid * .28f) *
                            (Pulse(t,.008f,19f) + Pulse(t,.11f,25f) * .65f);
                        break;
                    case 1: // Quill: three light scratches, no pitched confirmation beep.
                        signal = high * .27f * (Pulse(t,0f,48f) + Pulse(t,.065f,55f) * .7f + Pulse(t,.13f,48f) * .6f);
                        break;
                    case 2: // Wax seal impression: damped surface contact.
                        signal = (body * .28f + low * .65f + mid * .17f) * Pulse(t,0f,32f);
                        break;
                    case 3: // Order: quiet desk tap followed by a paper edge.
                        signal = (body * .13f + mid * .26f) * Pulse(t,0f,48f) + high * .12f * Pulse(t,.045f,38f);
                        break;
                    case 4: // Short cloth/boot suggestion, not a marching music loop.
                        signal = (low * .55f + body * .15f) * (Pulse(t,0f,29f) + Pulse(t,.16f,30f) * .7f + Pulse(t,.31f,40f) * .4f);
                        break;
                    case 5: // Weekly ledger: leaf turn and closing contact.
                        signal = (high * .15f + mid * .25f) * (Pulse(t,.01f,12f) + Pulse(t,.17f,17f) * .5f) +
                            (body * .10f + low * .3f) * Pulse(t,.31f,28f);
                        break;
                    case 6: // Restrained distant volley: staggered transients and an airy tail.
                        signal = (mid * .36f + low * .55f) * (Pulse(t,0f,43f) + Pulse(t,.03f,40f) * .8f + Pulse(t,.07f,34f) * .6f) +
                            low * .4f * Pulse(t,.07f,11f);
                        break;
                    case 7: // Soft impact; deliberately quieter than a volley.
                        signal = (mid * .26f + low * .4f) * Pulse(t,0f,48f);
                        break;
                    case 8: // Victory report: dry seal plus page movement; no musical fanfare.
                        signal = (body * .20f + mid * .3f) * Pulse(t,0f,29f) + high * .14f * Pulse(t,.12f,16f);
                        break;
                    default: // Defeat report: low desk contact and a trailing paper scrape.
                        signal = (body * .18f + low * .6f) * Pulse(t,0f,20f) + mid * .17f * Pulse(t,.14f,11f);
                        break;
                }
                // Short fades avoid boundary clicks; conservative ceiling prevents harsh peaks.
                float fade = Mathf.Min(1f,t / .004f) * Mathf.Min(1f,(length - t) / .025f);
                samples[i] = Mathf.Clamp(signal * fade,-.65f,.65f);
            }
            var clip = AudioClip.Create("Cabinet procedural " + CueNames[kind],count,1,SampleRate,false);
            clip.SetData(samples,0);
            return clip;
        }
    }
}

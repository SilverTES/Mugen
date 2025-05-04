using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Mugen.Audio
{
    public class SoundEffectManager
    {
        // Structure pour stocker les informations sur chaque effet sonore
        private class SoundEffectInfo
        {
            public int MaxConcurrentInstances { get; set; } // Nombre maximum d'instances simultanées
            public List<SoundEffectInstance> ActiveInstances { get; set; } // Liste des instances en cours

            public SoundEffectInfo(int maxConcurrentInstances)
            {
                MaxConcurrentInstances = maxConcurrentInstances;
                ActiveInstances = new List<SoundEffectInstance>();
            }
        }

        private readonly Dictionary<SoundEffect, SoundEffectInfo> _soundEffects;
        //private readonly Game _game;

        public SoundEffectManager()
        {
            _soundEffects = new Dictionary<SoundEffect, SoundEffectInfo>();
            //_game = game;
        }

        // Enregistrer un effet sonore avec une limite d'instances simultanées
        public void AddSoundEffect(SoundEffect soundEffect, int maxConcurrentInstances)
        {
            if (soundEffect == null)
                throw new ArgumentNullException(nameof(soundEffect));

            if (!_soundEffects.ContainsKey(soundEffect))
            {
                _soundEffects[soundEffect] = new SoundEffectInfo(maxConcurrentInstances);
            }
        }

        // Jouer un effet sonore si la limite n'est pas atteinte
        public SoundEffectInstance? Play(SoundEffect soundEffect, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if (soundEffect == null || !_soundEffects.ContainsKey(soundEffect))
            {
                return null; // Son non enregistré ou null
            }

            SoundEffectInfo info = _soundEffects[soundEffect];

            // Nettoyer les instances terminées
            info.ActiveInstances.RemoveAll(instance => instance.State == SoundState.Stopped);

            // Vérifier si on peut jouer une nouvelle instance
            if (info.ActiveInstances.Count >= info.MaxConcurrentInstances)
            {
                return null; // Limite atteinte, ne pas jouer
            }

            // Créer et jouer une nouvelle instance
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.Play();

            // Ajouter à la liste des instances actives
            info.ActiveInstances.Add(instance);

            return instance;
        }

        // Mettre à jour pour nettoyer les instances terminées (optionnel, appelé dans Game.Update)
        public void Update(GameTime gameTime)
        {
            foreach (var info in _soundEffects.Values)
            {
                info.ActiveInstances.RemoveAll(instance => instance.State == SoundState.Stopped);
            }
        }

        // Libérer toutes les instances et nettoyer
        public void Dispose()
        {
            foreach (var info in _soundEffects.Values)
            {
                foreach (var instance in info.ActiveInstances)
                {
                    instance?.Stop();
                    instance?.Dispose();
                }
                info.ActiveInstances.Clear();
            }
            _soundEffects.Clear();
        }
    }

}

using UnityEngine;

public static class GameConstants
{
    public static class Shaders
    {
        public const string SpritesDefault = "Sprites/Default";
        public static readonly int HitFlash = Shader.PropertyToID("_HitFlash");
        public static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    }

    public static class Tags
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Bullet = "Bullet";
    }

    public static class Layers
    {
        public const string Default = "Default";
        public const string Enemy = "Enemy";
    }

    public static class PoolTags
    {
        public const string BloodVFX = "BloodVFX";
    }
}

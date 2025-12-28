using UnityEngine;

public static class PoofParticles
{
    private static ParticleSystem template_white;
    private static ParticleSystem template_red;

    public static void EnsureCreatedWhite()
    {
        if (template_white != null) return;

        var go = new GameObject("PoofParticles_Template_White");
        go.hideFlags = HideFlags.HideAndDontSave;

        template_white = go.AddComponent<ParticleSystem>();

        // Configure the particle system
        var main = template_white.main;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.0f, 3.0f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 1f, 1f, 0.9f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;


        var emission = template_white.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 25)
        });

        var shape = template_white.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var colorOverLifetime = template_white.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            }
        );

        var sizeOverLifetime = template_white.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f)
        );

        // Renderer setup (billboard) + particle shader
        var psr = go.GetComponent<ParticleSystemRenderer>();
        psr.renderMode = ParticleSystemRenderMode.Billboard;
        var shader = Shader.Find("Particles/Standard Unlit");
        var mat = new Material(shader);
        psr.sharedMaterial = mat;
        
        
        // Don’t leave it running
        template_white.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    
    public static void EnsureCreatedRed()
    {
        if (template_red != null) return;

        var go = new GameObject("PoofParticles_Template_Red");
        go.hideFlags = HideFlags.HideAndDontSave;

        template_red = go.AddComponent<ParticleSystem>();

        // Configure the particle system
        var main = template_red.main;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.0f, 3.0f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 1f, 1f, 0.9f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;


        var emission = template_red.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 25)
        });

        var shape = template_red.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var colorOverLifetime = template_red.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.red, 1f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            }
        );

        var sizeOverLifetime = template_red.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f)
        );

        // Renderer setup (billboard) + particle shader
        var psr = go.GetComponent<ParticleSystemRenderer>();
        psr.renderMode = ParticleSystemRenderMode.Billboard;
        var shader = Shader.Find("Particles/Standard Unlit");
        var mat = new Material(shader);
        psr.sharedMaterial = mat;
        
        
        // Don’t leave it running
        template_red.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

        
    public static void SpawnWhite(Vector3 position)
    {
        EnsureCreatedWhite();

        var ps = Object.Instantiate(template_white, position, Quaternion.identity);
        ps.gameObject.hideFlags = HideFlags.DontSave;

        ps.Play();

        // Destroy after done
        float lifetime =
            ps.main.duration + ps.main.startLifetime.constantMax;
        Object.Destroy(ps.gameObject, lifetime + 0.1f);
    }
    
    public static void SpawnRed(Vector3 position)
    {
        EnsureCreatedRed();

        var ps = Object.Instantiate(template_red, position, Quaternion.identity);
        ps.gameObject.hideFlags = HideFlags.DontSave;

        ps.Play();

        // Destroy after done
        float lifetime =
            ps.main.duration + ps.main.startLifetime.constantMax;
        Object.Destroy(ps.gameObject, lifetime + 0.1f);
    }
    
}
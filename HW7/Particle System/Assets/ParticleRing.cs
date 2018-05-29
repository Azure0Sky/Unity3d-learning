using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( ParticleSystem ) )]

public class ParticleRing : MonoBehaviour {

    public ParticleSystem particleSys;
    public float radius = 50f;              // 总体半径
    public float speed = 0.2f;              // 总旋转速度

    readonly int maxParticleNum = 25000;
    ParticleSystem.Particle[] particles;
    float[] particlesRadius;
    float[] particlesAngle;

    void Start ()
    {
        if ( particleSys == null )
            particleSys = GetComponent<ParticleSystem>();

        var main = particleSys.main;
        main.maxParticles = maxParticleNum;

        if ( particles == null || particles.Length < particleSys.main.maxParticles )
            particles = new ParticleSystem.Particle[particleSys.main.maxParticles];

        particleSys.Emit( maxParticleNum );
        int numParticlesAlive = particleSys.GetParticles( particles );
        particlesRadius = new float[maxParticleNum];
        particlesAngle = new float[maxParticleNum];

        for ( int i = 0; i < numParticlesAlive; i++ ) {
            float pRadius = RandomNormal( radius, 2.5f );              // 正态分布的粒子运动半径，第二个参数为方差
            float angle = Random.Range( 0, 360f );

            particlesRadius[i] = pRadius;
            particlesAngle[i] = angle;
            particles[i].position = new Vector3( pRadius * Mathf.Cos( angle * Mathf.Deg2Rad ), 0, pRadius * Mathf.Sin( angle * Mathf.Deg2Rad ) );
        }

        particleSys.SetParticles( particles, numParticlesAlive );
    }

    void Update ()
    {
        for ( int i = 0; i < maxParticleNum; ++i ) {

            if ( i % 2 == 0 ) {                                     // 旋转方向
                particlesAngle[i] += speed * ( i % 3 + 1 );         // 三种不同的速度
                if ( particlesAngle[i] > 360 ) {
                    particlesAngle[i] -= 360;
                }
            } else {
                particlesAngle[i] -= speed * ( i % 3 + 1 );
                if ( particlesAngle[i] < 0 ) {
                    particlesAngle[i] += 360;
                }
            }

            float pRadius = particlesRadius[i] + Random.Range( -2f, 2f );
            float radian = particlesAngle[i] * Mathf.Deg2Rad;

            // 使粒子旋转
            Vector3 next = new Vector3( pRadius * Mathf.Cos( radian ), 0, pRadius * Mathf.Sin( radian ) );
            particles[i].position = Vector3.Slerp( particles[i].position, next, Time.deltaTime );

        }

        particleSys.SetParticles( particles, maxParticleNum );
    }
    
    // 正态分布
    private float RandomNormal( float expectation, float variance )
    {
        float s = 0, u = 0, v = 0;
        while ( s > 1 || s == 0 ) {
            u = Random.Range( -1f, 1f );
            v = Random.Range( -1f, 1f );
            s = u * u + v * v;
        }

        float normal = Mathf.Sqrt( -2 * Mathf.Log( s ) / s ) * u * variance + expectation;
        return normal;
    }
}

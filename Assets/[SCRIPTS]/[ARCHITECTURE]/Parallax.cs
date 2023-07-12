using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform followingTarget;
    [SerializeField][Range(0, 1f)] float parallaxStrength;
    [SerializeField] bool disableVericalParallax;
    Vector3 targetPreviousPosition;

    private void Start()
    {
        if (!followingTarget) followingTarget = Camera.main.transform;

        targetPreviousPosition = followingTarget.position;
    }

    private void Update()
    {
        Vector3 delta = followingTarget.position - targetPreviousPosition;

        if (disableVericalParallax) delta.y = 0;

        targetPreviousPosition = followingTarget.position;

        transform.position += delta * parallaxStrength;
    }
}

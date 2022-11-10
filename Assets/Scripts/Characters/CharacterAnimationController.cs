using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterAnimationController
{
    private Animator _animator;

    public enum Animations
    {
        Attack,
        Walk,
        Run
    }

    public Dictionary<Animations, string> animations = new();
    public CharacterAnimationController(Animator animator)
    {
        _animator = animator;

        foreach (var val in Enum.GetValues(typeof(Animations)))
            animations.Add((Animations)val, ((Animations)val).ToString());

    }

    public void SetAnimation(Animations animation, bool value = true)
    {
        if (value)
            _animator.SetTrigger(animations[animation]);
        else
            _animator.ResetTrigger(animations[animation]);
    }

}

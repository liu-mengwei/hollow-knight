using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimateEvents : MonoBehaviour
{
    // Start is called before the first frame update
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void AnimationTrigger() { 
        player.attackOver();
    }
}

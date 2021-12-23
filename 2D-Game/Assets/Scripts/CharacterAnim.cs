using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterAnim : MonoBehaviour
{
    PhotonView view;
    private Animator anim;
    void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (view.IsMine) {
            if ((Input.GetAxis("Horizontal")) != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                anim.SetTrigger("Jump");
            }
        } 
    }
}

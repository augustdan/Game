using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerDaya, string animBoolName) : base(player, stateMachine, playerDaya, animBoolName)
    {
    }
}
    


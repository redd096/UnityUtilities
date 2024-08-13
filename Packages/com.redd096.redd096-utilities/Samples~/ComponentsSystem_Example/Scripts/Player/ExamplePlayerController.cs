using redd096.v2.ComponentsSystem;
using UnityEngine;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// Generally you want to calculate inputs inside InputManager attached to this same GameObject, because this is always instantiate for every player, even online. 
    /// Then your Pawn in scene will read the inputs. Or in a multiplayer online, your pawn on the server will read inputs and syncronize position, rotation, etc... with client
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example Player Controller")]
    public class ExamplePlayerController : PlayerController
    {
    }
}
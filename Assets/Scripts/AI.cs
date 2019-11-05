using System.Collections.Generic;
using UnityEngine;

/*****************************************************************************************************************************
 * Write your core AI code in this file here. The private variable 'agentScript' contains all the agents actions which are listed
 * below. Ensure your code it clear and organised and commented.
 *
 * Unity Tags
 * public static class Tags
 * public const string BlueTeam = "Blue Team";	The tag assigned to blue team members.
 * public const string RedTeam = "Red Team";	The tag assigned to red team members.
 * public const string Collectable = "Collectable";	The tag assigned to collectable items (health kit and power up).
 * public const string Flag = "Flag";	The tag assigned to the flags, blue or red.
 * 
 * Unity GameObject names
 * public static class Names
 * public const string PowerUp = "Power Up";	Power up name
 * public const string HealthKit = "Health Kit";	Health kit name.
 * public const string BlueFlag = "Blue Flag";	The blue teams flag name.
 * public const string RedFlag = "Red Flag";	The red teams flag name.
 * public const string RedBase = "Red Base";    The red teams base name.
 * public const string BlueBase = "Blue Base";  The blue teams base name.
 * public const string BlueTeamMember1 = "Blue Team Member 1";	Blue team member 1 name.
 * public const string BlueTeamMember2 = "Blue Team Member 2";	Blue team member 2 name.
 * public const string BlueTeamMember3 = "Blue Team Member 3";	Blue team member 3 name.
 * public const string RedTeamMember1 = "Red Team Member 1";	Red team member 1 name.
 * public const string RedTeamMember2 = "Red Team Member 2";	Red team member 2 name.
 * public const string RedTeamMember3 = "Red Team Member 3";	Red team member 3 name.
 * 
 * _agentData properties and public variables
 * public bool IsPoweredUp;	Have we powered up, true if we’re powered up, false otherwise.
 * public int CurrentHitPoints;	Our current hit points as an integer
 * public bool HasFriendlyFlag;	True if we have collected our own flag
 * public bool HasEnemyFlag;	True if we have collected the enemy flag
 * public GameObject FriendlyBase; The friendly base GameObject
 * public GameObject EnemyBase;    The enemy base GameObject
 * public int FriendlyScore; The friendly teams score
 * public int EnemyScore;       The enemy teams score
 * 
 * _agentActions methods
 * public bool MoveTo(GameObject target)	Move towards a target object. Takes a GameObject representing the target object as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveTo(Vector3 target)	Move towards a target location. Takes a Vector3 representing the destination as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveToRandomLocation()	Move to a random location on the level, returns true if the location is on the navmesh, false otherwise.
 * public void CollectItem(GameObject item)	Pick up an item from the level which is within reach of the agent and put it in the inventory. Takes a GameObject representing the item as a parameter.
 * public void DropItem(GameObject item)	Drop an inventory item or the flag at the agents’ location. Takes a GameObject representing the item as a parameter.
 * public void UseItem(GameObject item)	Use an item in the inventory (currently only health kit or power up). Takes a GameObject representing the item to use as a parameter.
 * public void AttackEnemy(GameObject enemy)	Attack the enemy if they are close enough. ). Takes a GameObject representing the enemy as a parameter.
 * public void Flee(GameObject enemy)	Move in the opposite direction to the enemy. Takes a GameObject representing the enemy as a parameter.
 * 
 * _agentSenses properties and methods
 * public List<GameObject> GetObjectsInViewByTag(string tag)	Return a list of objects with the same tag. Takes a string representing the Unity tag. Returns null if no objects with the specified tag are in view.
 * public GameObject GetObjectInViewByName(string name)	Returns a specific GameObject by name in view range. Takes a string representing the objects Unity name as a parameter. Returns null if named object is not in view.
 * public List<GameObject> GetObjectsInView()	Returns a list of objects within view range. Returns null if no objects are in view.
 * public List<GameObject> GetCollectablesInView()	Returns a list of objects with the tag Collectable within view range. Returns null if no collectable objects are in view.
 * public List<GameObject> GetFriendliesInView()	Returns a list of friendly team AI agents within view range. Returns null if no friendlies are in view.
 * public List<GameObject> GetEnemiesInView()	Returns a list of enemy team AI agents within view range. Returns null if no enemy are in view.
 * public bool IsItemInReach(GameObject item)	Checks if we are close enough to a specific collectible item to pick it up), returns true if the object is close enough, false otherwise.
 * public bool IsInAttackRange(GameObject target)	Check if we're with attacking range of the target), returns true if the target is in range, false otherwise.
 * 
 * _agentInventory properties and methods
 * public bool AddItem(GameObject item)	Adds an item to the inventory if there's enough room (max capacity is 'Constants.InventorySize'), returns true if the item has been successfully added to the inventory, false otherwise.
 * public GameObject GetItem(string itemName)	Retrieves an item from the inventory as a GameObject, returns null if the item is not in the inventory.
 * public bool HasItem(string itemName)	Checks if an item is stored in the inventory, returns true if the item is in the inventory, false otherwise.
 * 
 * You can use the game objects name to access a GameObject from the sensing system. Thereafter all methods require the GameObject as a parameter.
 * 
*****************************************************************************************************************************/

/// <summary>
/// Implement your AI script here, you can put everything in this file, or better still, break your code up into multiple files
/// and call your script here in the Update method. This class includes all the data members you need to control your AI agent
/// get information about the world, manage the AI inventory and access essential information about your AI.
///
/// You may use any AI algorithm you like, but please try to write your code properly and professionaly and don't use code obtained from
/// other sources, including the labs.
///
/// See the assessment brief for more details
/// </summary>
public class AI : MonoBehaviour
{
    // Gives access to important data about the AI agent (see above)
    private AgentData _agentData;
    // Gives access to the agent senses
    private Sensing _agentSenses;
    // gives access to the agents inventory
    private InventoryController _agentInventory;
    // This is the script containing the AI agents actions
    // e.g. agentScript.MoveTo(enemy);
    private AgentActions _agentActions;


    public bool Leader;
    GameObject LeaderObj;
    public GameObject TargetsObj;

    public int State = 0;


    // Use this for initialization
    void Start()
    {
        // Initialise the accessable script components
        _agentData = GetComponent<AgentData>();
        _agentActions = GetComponent<AgentActions>();
        _agentSenses = GetComponentInChildren<Sensing>();
        _agentInventory = GetComponentInChildren<InventoryController>();

        if (gameObject.name.Contains("1"))
        {
            Leader = true;
            LeaderObj = gameObject;
        }
        else
        {
            Leader = false;
            string FindLeader = gameObject.tag + " Member 1";
            LeaderObj = GameObject.Find(FindLeader);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case 0: //Default Starting State

                StateChange(); //checks for changes to state

                break;

            case 1:// move toward base

                _agentActions.MoveTo(_agentData.EnemyBase); //move to enemy base
                FindTarget(); // finds a target
                StateChange(); //checks for changes to state

                break;

            case 2:// move toward Enemy and Swing

                if (TargetsObj != null)
                {
                    _agentActions.MoveTo(TargetsObj); // move towards enemy
                    _agentActions.AttackEnemy(TargetsObj); // swing
                }

               StateChange();

                break;

            case 3:// collect flag

                _agentActions.MoveTo(_agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName)); //goes to flag when in sight
                _agentActions.CollectItem(_agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName)); //collects flag
                StateChange();

                break;

            case 4:// return Flag

                _agentActions.MoveTo(_agentData.FriendlyBase); //goes back to base

                if (Vector3.Distance(transform.position, _agentData.FriendlyBase.transform.position) < 5)
                {
                    GameObject flag = _agentInventory.GetItem(_agentData.EnemyFlagName);
                    _agentActions.DropItem(flag);
                    SetAllTeamStates(0);
                }

                StateChange();

                break;

            case 5:// follow flag

                _agentActions.MoveTo(_agentData.FriendlyBase);
                GotFlagCheck();

                break;

            case 6: // Defend

                _agentActions.MoveTo(_agentData.FriendlyBase); //move to enemy base
                FindTarget(); // finds a target
                StateChange(); //checks for changes to state
                break;

            case 7:

                if (_agentData.CurrentHitPoints < 50)
                {
                    _agentActions.Flee(TargetsObj);
                }
                StateChange();
                break;




        }



       

        //_agentActions.CollectItem(GameObject.Find("Power Up")); //collects power up

        //_agentActions.CollectItem(GameObject.Find("Health Kit")); //collects health kit


    }

    void FindTarget()
    {
        if (Leader == true)
        {
            float Distance = 9999999; // gets new target
            List<GameObject> Enemys = _agentSenses.GetEnemiesInView();
            foreach (GameObject target in Enemys)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < Distance)
                {
                    Distance = Vector3.Distance(transform.position, target.transform.position);
                    TargetsObj = target;
                }
            }
        }
        else if (LeaderObj != null)
        {
            TargetsObj = LeaderObj.GetComponent<AI>().TargetsObj; // gets target from leader
        }
        else
        {
            string FindLeader = gameObject.tag + " Member 1";
            LeaderObj = GameObject.Find(FindLeader); //finds new leader if there is one

            float Distance = 9999999; // gets new target
            List<GameObject> Enemys = _agentSenses.GetEnemiesInView();
            foreach (GameObject target in Enemys)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < Distance)
                {
                    Distance = Vector3.Distance(transform.position, target.transform.position);
                    TargetsObj = target;
                }
            }
        }

    }

    void StateChange() //checks for changes needed for states
    {

        if (_agentInventory.HasItem(_agentData.EnemyFlagName))//go back if have flag
        {
            SetAllTeamStates(5);
            State = 4;
        }
        else if (_agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName) && Vector3.Distance(_agentData.FriendlyBase.transform.position, _agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName).transform.position) > 5) //go get the flag if can see it
        {
            State = 3;
        }
        else if (_agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName) && Vector3.Distance(_agentData.FriendlyBase.transform.position, _agentSenses.GetObjectInViewByName(_agentData.EnemyFlagName).transform.position) <= 5 && TargetsObj == null)// defend the flag
        {
            State = 6;
        }
        else if (TargetsObj == null) //go to enemy base
        {
            State = 1;
        }
        else if (TargetsObj != null) //kill enemy
        {
            State = 2;
        }
        else if (_agentData.CurrentHitPoints < 50)
        {
            State = 7;
        }

    }

    void GotFlagCheck()// checks if any team mate has the flag
    {
        GameObject temp1 = GameObject.Find(gameObject.tag + " Member 1");// gets team mates
        GameObject temp2 = GameObject.Find(gameObject.tag + " Member 2");
        GameObject temp3 = GameObject.Find(gameObject.tag + " Member 3");

        int state = 0;

        if (temp1 != null) // checks for flag
        {
            if (temp1.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                state = 5;
            }
        }
        if (temp2 != null)
        {
            if (temp2.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                state = 5;
            }
        }
        if (temp3 != null)
        {
            if (temp3.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                state = 5;
            }
        }


        State = state;

    }

    void SetAllTeamStates(int state) //set all teams ai to a state
    {
        GameObject temp1 = GameObject.Find(gameObject.tag + " Member 1");// gets team
        GameObject temp2 = GameObject.Find(gameObject.tag + " Member 2");
        GameObject temp3 = GameObject.Find(gameObject.tag + " Member 3");


        if (temp1 != null)//sets states
        {
            temp1.GetComponent<AI>().State = state;
        }
        if (temp2 != null)
        {
            temp2.GetComponent<AI>().State = state;
        }
        if (temp3 != null)
        {
            temp3.GetComponent<AI>().State = state;
        }
    }
}
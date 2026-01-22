using UnityEngine;
using StateMachine;

namespace StateMachine.Examples
{
    /// <summary>
    /// Example implementation showing how to use the state machine system
    /// This example creates a simple AI character with Idle, Patrol, and Chase states
    /// </summary>
    public class ExampleAI : StateMachineBehaviour
    {
        [Header("AI Settings")]
        public Transform target;
        public float detectionRange = 10f;
        public float moveSpeed = 3f;
        public Transform[] patrolPoints;
        
        [Header("Debug")]
        public bool showGizmos = true;
        
        private void Start()
        {
            // Create and register states
            var idleState = new IdleState(StateMachine, this);
            var patrolState = new PatrolState(StateMachine, this);
            var chaseState = new ChaseState(StateMachine, this);
            
            RegisterStates(idleState, patrolState, chaseState);
            
            // If using AdvancedStateMachine, you can also set up automatic transitions
            if (StateMachine is AdvancedStateMachine advancedSM)
            {
                // Transition from Idle to Patrol when no target is detected
                advancedSM.AddTransition<IdleState, PatrolState>(() => !IsTargetInRange());
                
                // Transition from any state to Chase when target is detected
                advancedSM.AddGlobalTransition<ChaseState>(() => IsTargetInRange());
                
                // Transition from Chase back to Idle when target is lost
                advancedSM.AddTransition<ChaseState, IdleState>(() => !IsTargetInRange());
            }
            
            // Start with idle state
            StartStateMachine<IdleState>();
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, detectionRange);
            
            // Draw patrol points
            if (patrolPoints != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one);
                        
                        // Draw lines between patrol points
                        if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        }
                    }
                }
            }
        }
        
        public bool IsTargetInRange()
        {
            if (target == null) return false;
            return Vector3.Distance(transform.position, target.position) <= detectionRange;
        }
        
        public void MoveTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            // Rotate to face movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    
    /// <summary>
    /// Example idle state
    /// </summary>
    public class IdleState : BaseState
    {
        private ExampleAI aiController;
        private float idleTimer;
        private const float MAX_IDLE_TIME = 3f;
        
        public IdleState(StateMachine stateMachine, ExampleAI aiController) 
            : base(stateMachine, "Idle")
        {
            this.aiController = aiController;
        }
        
        public override void OnEnter()
        {
            Debug.Log("AI: Entering Idle State");
            idleTimer = 0f;
        }
        
        public override void OnUpdate()
        {
            idleTimer += Time.deltaTime;
            
            // Manually transition to patrol after some time (if not using AdvancedStateMachine)
            if (!(stateMachine is AdvancedStateMachine) && idleTimer >= MAX_IDLE_TIME)
            {
                if (!aiController.IsTargetInRange())
                {
                    TransitionTo<PatrolState>();
                }
            }
        }
        
        public override void OnExit()
        {
            Debug.Log("AI: Exiting Idle State");
        }
    }
    
    /// <summary>
    /// Example patrol state
    /// </summary>
    public class PatrolState : BaseState
    {
        private ExampleAI aiController;
        private int currentPatrolIndex = 0;
        
        public PatrolState(StateMachine stateMachine, ExampleAI aiController) 
            : base(stateMachine, "Patrol")
        {
            this.aiController = aiController;
        }
        
        public override void OnEnter()
        {
            Debug.Log("AI: Entering Patrol State");
        }
        
        public override void OnUpdate()
        {
            if (aiController.patrolPoints == null || aiController.patrolPoints.Length == 0)
            {
                TransitionTo<IdleState>();
                return;
            }
            
            // Move towards current patrol point
            Transform currentPatrolPoint = aiController.patrolPoints[currentPatrolIndex];
            if (currentPatrolPoint != null)
            {
                aiController.MoveTowards(currentPatrolPoint.position);
                
                // Check if we've reached the patrol point
                if (Vector3.Distance(aiController.transform.position, currentPatrolPoint.position) < 0.5f)
                {
                    // Move to next patrol point
                    currentPatrolIndex = (currentPatrolIndex + 1) % aiController.patrolPoints.Length;
                }
            }
            
            // Manual transition check (if not using AdvancedStateMachine)
            if (!(stateMachine is AdvancedStateMachine) && aiController.IsTargetInRange())
            {
                TransitionTo<ChaseState>();
            }
        }
        
        public override void OnExit()
        {
            Debug.Log("AI: Exiting Patrol State");
        }
    }
    
    /// <summary>
    /// Example chase state
    /// </summary>
    public class ChaseState : BaseState
    {
        private ExampleAI aiController;
        
        public ChaseState(StateMachine stateMachine, ExampleAI aiController) 
            : base(stateMachine, "Chase")
        {
            this.aiController = aiController;
        }
        
        public override void OnEnter()
        {
            Debug.Log("AI: Entering Chase State");
        }
        
        public override void OnUpdate()
        {
            if (aiController.target != null)
            {
                // Move towards target at double speed
                Vector3 targetPos = aiController.target.position;
                Vector3 direction = (targetPos - aiController.transform.position).normalized;
                aiController.transform.position += direction * (aiController.moveSpeed * 2f) * Time.deltaTime;
                
                // Rotate to face target
                if (direction != Vector3.zero)
                {
                    aiController.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            
            // Manual transition check (if not using AdvancedStateMachine)
            if (!(stateMachine is AdvancedStateMachine) && !aiController.IsTargetInRange())
            {
                TransitionTo<IdleState>();
            }
        }
        
        public override void OnExit()
        {
            Debug.Log("AI: Exiting Chase State");
        }
    }
}
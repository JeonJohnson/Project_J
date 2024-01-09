using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using Structs;

namespace MoreMountains.TopDownEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Suck 2D")]
    public class CharacterSuck2D : CharacterAbility
    {
        /// This method is only used to display a helpbox text
        /// at the beginning of the ability's inspector
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        [Header("TODO_HEADER")]
        /// declare your parameters here
        public float randomParameter = 4f;
        public bool randomBool;

        protected const string _yourAbilityAnimationParameterName = "YourAnimationParameterName";
        protected int _yourAbilityAnimationParameter;

        [System.Serializable]
        public struct SuctionStat
        {
            public Color fovIdleColor;
            public Color fovSuctionColor;
            public float curSuctionRatio;
            public float maxSuctionTime;
            public float rechargeTime;
            public float suctionAngle;
            public float suctionRange;

            public LayerMask targetLayer;
        }

        private Player owner;
        public SuctionStat suctionStat;
        public Transform suctionGunTr;
        public SpriteRenderer fovSprite;

        private CharacterHandleWeapon characterHandleWeapon;
        private CharacterOrientation2D characterOrientation2D;


        /// <summary>
        /// Here you should initialize our parameters
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            randomBool = false;
            characterHandleWeapon = _character?.FindAbility<CharacterHandleWeapon>();
            suctionGunTr.parent = characterHandleWeapon.CurrentWeapon.transform;
            suctionGunTr.localEulerAngles = Vector3.zero;
            fovSprite.material.SetFloat("_FovAngle", suctionStat.suctionAngle);
            fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
        }

        /// <summary>
        /// Every frame, we check if we're crouched and if we still should be
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// Called at the start of the ability's cycle, this is where you'll check for input
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if(AbilityAuthorized)
            {
                if(Input.GetKey(KeyCode.Mouse1))
                {
                    suctionGunTr.gameObject.SetActive(true);
                  //  suctionGunTr.rotation = characterHandleWeapon.CurrentWeapon.transform.rotation;
                }
                else
                {
                    suctionGunTr.gameObject.SetActive(false);
                }
                Suction();
            }
        }


        /// <summary>
        /// If we're pressing down, we check for a few conditions to see if we can perform our action
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (!_controller.Grounded))
            {
                // we do nothing and exit
                return;
            }

            // if we're still here, we display a text log in the console
            MMDebug.DebugLogTime("We're doing something yay!");
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_yourAbilityAnimationParameterName, AnimatorControllerParameterType.Bool, out _yourAbilityAnimationParameter);
        }

        /// <summary>
        /// At the end of the ability's cycle,
        /// we send our current crouching and crawling states to the animator
        /// </summary>
        public override void UpdateAnimator()
        {

            bool myCondition = true;
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _yourAbilityAnimationParameter, myCondition, _character._animatorParameters);
        }


        public void Suction()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (suctionStat.curSuctionRatio <= 0f)
                {
                    //Recharging();
                    fovSprite.color = this.suctionStat.fovIdleColor;
                    return;
                }

                float amount = (1f / suctionStat.maxSuctionTime) * Time.deltaTime;

                if (suctionStat.curSuctionRatio < amount)
                {
                    //Recharging();
                    fovSprite.color = suctionStat.fovIdleColor;
                    return;
                }
                suctionStat.curSuctionRatio = Mathf.Clamp(suctionStat.curSuctionRatio - amount, 0f, 1f);
                fovSprite.color = suctionStat.fovSuctionColor;
                var cols = Physics2D.OverlapCircleAll(suctionGunTr.position, suctionStat.suctionRange, suctionStat.targetLayer);

                foreach (var col in cols)
                {
                    Vector3 targetPos = col.transform.position;
                    Vector2 targetDir = (targetPos - suctionGunTr.position).normalized;

                    var tempLookDir = Funcs.DegreeAngle2Dir(-suctionGunTr.eulerAngles.z);
                    //lookDir랑 값다른데 이거로 적용됨 일단 나중에 ㄱ
                    float angleToTarget = Mathf.Acos(Vector2.Dot(targetDir, tempLookDir)) * Mathf.Rad2Deg;

                    //내적해주고 나온 라디안 각도를 역코사인걸어주고 오일러각도로 변환.
                    if (angleToTarget <= (suctionStat.suctionAngle))
                    {
                        //여기서 총알들 한테 흡수 ㄱ
                        Projectile projectile = col.gameObject.GetComponent<Projectile>();
                        if (projectile)
                        {
                            Debug.Log("올");
                            projectile.transform.SetParent(null);
                            if (projectile.suckedOption == Projectile.SuckedOption.Sucked && projectile.curState == Projectile.BulletState.Fire)
                            {
                                projectile.Sucked(characterHandleWeapon.CurrentWeapon);
                            }
                        }
                    }
                }
            }
            else
            {
                fovSprite.color = this.suctionStat.fovIdleColor;
                Recharge();
            }
        }

        public void Recharge()
        {
            suctionStat.curSuctionRatio = Mathf.Clamp(suctionStat.curSuctionRatio + (1 / suctionStat.rechargeTime * Time.deltaTime), 0f, 1f);
        }
    }
}
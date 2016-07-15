using CocosSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WarOfLords.Common;
using WarOfLords.Common.Models;

namespace WarOfLords.Client
{
    class BattleUnitSprite: CCSprite
    {
        BattleUnit battleUnit;
        int teamId;
        public ConcurrentQueue<CCAction> actionQ;
        private object moveActionLock = new object();
        private CancellationTokenSource spriteCancelSource;

        public BattleUnitSprite(BattleUnit unit, int id)
        {
            actionQ = new ConcurrentQueue<CCAction>();
            battleUnit = unit;
            teamId = id;
            battleUnit.OnHealthChanged += this.OnBattleUnitHealthChanged;
            battleUnit.OnPositionChanged += this.OnBattleUnitPositionChanged;
            battleUnit.OnPositionInited += this.OnBattleUnitPositionInited;
            var texture = new CCTexture2D(getFileFromUnitType() + "-" + teamId);
            this.SpriteFrame = new CCSpriteFrame(texture, new CCRect(0, 0, texture.PixelsWide, texture.PixelsHigh));
            this.AnchorPoint = new CCPoint(0.5f, 0.5f);
            spriteCancelSource = new CancellationTokenSource();
            this.runAsync(spriteCancelSource.Token);
        }

     void OnBattleUnitHealthChanged(int from, int to)
        {
            if (this.battleUnit.Health <= 0)
            {
                this.spriteCancelSource.Cancel();

                //var deadTexture = new CCTexture2D("Dead");
                //this.ReplaceTexture(deadTexture, new CCRect(0, 0, deadTexture.PixelsWide, deadTexture.PixelsHigh));

                //this.battleUnit.OnHealthChanged -= OnBattleUnitHealthChanged;
                //this.battleUnit.OnPositionInited -= OnBattleUnitPositionInited;
                //this.battleUnit.OnPositionInited -= OnBattleUnitPositionInited;
                //this.RemoveFromParent();
            }
        }

         void  OnBattleUnitPositionChanged(MapVertex orgPos, MapVertex movedTo, TimeSpan duration)
        {
            CCMoveTo moveToAction = new CCMoveTo((float)duration.TotalSeconds, new CCPoint(movedTo.X, movedTo.Y));
            this.actionQ.Enqueue(moveToAction);
        }

        void OnBattleUnitPositionInited(MapVertex initPos)
        {
            this.Position = new CCPoint(initPos.X, initPos.Y);
        }

        string getFileFromUnitType()
        {
            if (battleUnit is SwordMan) return "SwordMan";
            if (battleUnit is BowMan) return "BowMan";
            if (battleUnit is MedicalMan) return "MedicalMan";
            if (battleUnit is Trebuchet) return "Trebuchet";
            if (battleUnit is Scout) return "Scout";
            if (battleUnit is WeaponOperator) return "WeaponOperator";
            return "BattleUnit";
        }

        async Task runAsync(CancellationToken cancelToken)
        {
            try
            {
                while (this.battleUnit.Health > 0 && !cancelToken.IsCancellationRequested)
                {
                    try
                    {
                        
                        CCAction action;
                        while (this.actionQ.TryDequeue(out action))
                        {
                            this.AddAction(action);
                        }
                        await Task.Delay(100);
                    }
                    catch { };
                }

                if (this.battleUnit.Health <= 0)
                {
                    //var deadTexture = new CCTexture2D("Dead");
                    //this.ReplaceTexture(deadTexture, new CCRect(0, 0, deadTexture.PixelsWide, deadTexture.PixelsHigh));

                    this.battleUnit.OnHealthChanged -= OnBattleUnitHealthChanged;
                    this.battleUnit.OnPositionInited -= OnBattleUnitPositionInited;
                    this.battleUnit.OnPositionInited -= OnBattleUnitPositionInited;
                    this.RemoveFromParent();
                }
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}

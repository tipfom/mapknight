using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Combat.Collections.Abilities;
using mapKnight.Extended.Graphics.Animation;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat.Collections.Secondaries {
    public class Shield : SecondaryWeapon {
        private static readonly VertexAnimationData ANIMATION_DATA = new VertexAnimationData( ) {
            Scales = new float[ ] {
                0.08244598f,
            },
            Animations = new VertexAnimation[ ] {
                new VertexAnimation( ) {
                    Name = "Idle",
                    CanRepeat = true,
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 500,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.3595486f, -0.008622517f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 800,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.3908814f, -0.05652778f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 600,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4110003f, -0.01739316f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Run",
                    CanRepeat = true,
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 100,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4081261f, -0.006522436f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 100,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4167485f, 0.0119578f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 130,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4110003f, -0.02500267f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 130,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4383045f, -0.01304487f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 170,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4627346f, -0.1619738f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Hit",
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 180,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.356695f, -0.006466888f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 100,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.3652557f, -0.2112517f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 170,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.402352f, -0.3060994f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 1,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4052055f, -0.267298f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Jump",
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 75,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4223269f, -0.2737649f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 75,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.428034f, -0.2651424f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 5,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4651303f, -0.1077815f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Fall",
                    CanRepeat = true,
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 180,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4708374f, -0.133649f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 180,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.5421764f, -0.09053643f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 180,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.5250551f, -0.1487384f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Hit2",
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 200,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.428034f, -0.2198742f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 110,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4793981f, -0.2371192f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 150,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.473691f, -0.02155629f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 1,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4337411f, -0.1271821f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
                new VertexAnimation( ) {
                    Name = "Hit3",
                    Frames = new VertexAnimationFrame[ ] {
                        new VertexAnimationFrame( ) {
                            Time = 200,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4365947f, -0.1120927f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 90,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.4423018f, 0.0150894f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 150,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(-0.188335f, -0.2284967f),
                                    Texture = "Shield"
                                },
                            }
                        },
                        new VertexAnimationFrame( ) {
                            Time = 1,
                            State = new VertexBone[ ] {
                                new VertexBone( ) {
                                    Position = new Vector2(0.356695f, -0.2349636f),
                                    Texture = "Shield"
                                },
                            }
                        },
                    },
                },
            },
            Offsets = new Vector2[ ]{
                new Vector2(10, 15)
            }
        };

        private Buff buffAbility;
        private Dash dashAbility;
        private Heal healAbility;
        private Terrorfield terrorFieldAbility;
        private Doom doomAbility;

        public Shield (Entity Owner) : base(Owner, "secondary_weapons/shield", ANIMATION_DATA) {
            buffAbility = new Buff(this);
            dashAbility = new Dash(this);
            healAbility = new Heal(this);
            terrorFieldAbility = new Terrorfield(this);
            doomAbility = new Doom(this);
        }

        public override IEnumerable<Ability> Abilities ( ) {
            //yield return buffAbility;
            yield return dashAbility;
            yield return healAbility;
            yield return doomAbility;
        }

        public override void Prepare ( ) {
            foreach (Ability ability in Abilities( ))
                ability.Prepare( );
        }

        public override void Update (DeltaTime dt) {
            buffAbility.Update(dt);
            dashAbility.Update(dt);
            healAbility.Update(dt);
            terrorFieldAbility.Update(dt);
            doomAbility.Update(dt);
        }
    }
}

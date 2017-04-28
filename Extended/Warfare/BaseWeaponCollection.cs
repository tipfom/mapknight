using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.Animation;

namespace mapKnight.Extended.Warfare {
    public static class BaseWeaponCollection {
        public static BaseWeapon DiamondSword (Entity owner) {
            return new BaseWeapon(
                "Diamond Sword", 0, 3f, 1000, "swords/diamond", 180,
                new VertexAnimationData( ) {
                    Offsets = new Vector2[ ] {
                        new Vector2(3, 25),     // sword
                        new Vector2(2, 2),       // hand1
                    },
                    Scales = new float[ ] {
                        0.04870608f,
                        0.04870608f,
                    },
                    Animations = new VertexAnimation[ ] {
                        new VertexAnimation( ) {
                            Name = "idle",
                            CanRepeat = true,
                            Frames = new VertexAnimationFrame[ ] {
                                new VertexAnimationFrame( ) {
                                    Time = 500,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -47f,
                                            Position = new Vector2(-0.3853965f, -0.2385302f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 800,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -45f,
                                            Position = new Vector2(-0.3896378f, -0.2748923f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = 2f,
                                            Position = new Vector2(-0.3575646f, -0.2895388f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 600,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -51f,
                                            Position = new Vector2(-0.3933463f, -0.2460565f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -11f,
                                            Position = new Vector2(-0.3644413f, -0.2555707f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                            },
                        },
                        new VertexAnimation( ) {
                            Name = "attack",
                            Frames = new VertexAnimationFrame[ ] {
                                new VertexAnimationFrame( ) {
                                    Time = 180,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -47f,
                                            Position = new Vector2(-0.3853965f, -0.2385302f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 100,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = 26f,
                                            Position = new Vector2(-0.1890052f, -0.1126988f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = 75f,
                                            Position = new Vector2(-0.1681619f, -0.07521598f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 170,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -56f,
                                            Position = new Vector2(-0.09572338f, -0.158121f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -12f,
                                            Position = new Vector2(-0.0404874f, -0.1699104f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 1,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -47f,
                                            Position = new Vector2(-0.3853965f, -0.2385302f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                            },
                        },
                        new VertexAnimation( ) {
                            Name = "walk",
                            CanRepeat = true,
                            Frames = new VertexAnimationFrame[ ] {
                                new VertexAnimationFrame( ) {
                                    Time = 100,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -47f,
                                            Position = new Vector2(-0.388224f, -0.2363913f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 100,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -33f,
                                            Position = new Vector2(-0.3180811f, -0.2287672f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = 13f,
                                            Position = new Vector2(-0.2788909f, -0.2273623f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 130,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -41f,
                                            Position = new Vector2(-0.2834927f, -0.2074512f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -2f,
                                            Position = new Vector2(-0.2446563f, -0.203935f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 130,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -44f,
                                            Position = new Vector2(-0.4616272f, -0.2384659f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -5f,
                                            Position = new Vector2(-0.4143082f, -0.2381582f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 170,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -49f,
                                            Position = new Vector2(-0.5181778f, -0.2363269f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -9f,
                                            Position = new Vector2(-0.46379f, -0.2338803f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                            },
                        },
                        new VertexAnimation( ) {
                            Name = "jump",
                            Frames = new VertexAnimationFrame[ ] {
                                new VertexAnimationFrame( ) {
                                    Time = 75,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -47f,
                                            Position = new Vector2(-0.3853965f, -0.2385302f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 75,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -51f,
                                            Position = new Vector2(-0.3853965f, -0.2385302f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -4f,
                                            Position = new Vector2(-0.3575646f, -0.2382041f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 5,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -56f,
                                            Position = new Vector2(-0.3627762f, -0.1444166f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -9f,
                                            Position = new Vector2(-0.3405994f, -0.1515768f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                            },
                        },
                        new VertexAnimation( ) {
                            Name = "fall",
                            CanRepeat = true,
                            Frames = new VertexAnimationFrame[ ] {
                                new VertexAnimationFrame( ) {
                                    Time = 180,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -56f,
                                            Position = new Vector2(-0.3627762f, -0.1444166f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -9f,
                                            Position = new Vector2(-0.3405994f, -0.1515768f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 180,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -72f,
                                            Position = new Vector2(-0.4989638f, -0.1163473f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -33f,
                                            Position = new Vector2(-0.4661487f, -0.1288491f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                                new VertexAnimationFrame( ) {
                                    Time = 180,
                                    State = new VertexBone[ ] {
                                        new VertexBone( ) {
                                            Rotation = -76f,
                                            Position = new Vector2(-0.3008564f, -0.1302504f),
                                            Texture = "sword"
                                        },
                                        new VertexBone( ) {
                                            Rotation = -26f,
                                            Position = new Vector2(-0.2680414f, -0.1542533f),
                                            Texture = "hand1"
                                        },
                                    }
                                },
                            },
                        },
                    }
                }, 
                new Transform(new Vector2(owner.Transform.Width, 0f), 
                new Vector2(owner.Transform.Width, owner.Transform.Height)),
                owner);
        }
    }
}

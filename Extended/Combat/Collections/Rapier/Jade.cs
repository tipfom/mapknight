using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.Animation;

namespace mapKnight.Extended.Combat.Collections
{
    public static partial class Rapier
    {
        public static PrimaryWeapon Jade (Entity owner) {
            return new PrimaryWeapon(
                "Jade Rapier", 2, 1.5f, 100, "weapons/rapier/jade", 100,
                new VertexAnimationData( ) {
                    Offsets = new Vector2[ ] {
                            new Vector2(2, 23),  // rapier
                            new Vector2(2, 2),       // hand1
                    },
                    Scales = new float[ ] {
                            0.08244598f,
                            0.08244598f,
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
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 800,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -45f,
                                                Position = new Vector2(-0.6632737f, -0.2748923f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 2f,
                                                Position = new Vector2(-0.6089827f, -0.2895388f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 600,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -51f,
                                                Position = new Vector2(-0.6695512f, -0.2460565f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -11f,
                                                Position = new Vector2(-0.620623f, -0.2555707f),
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
                                                Position = new Vector2(-0.6608806f, -0.2363913f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -33f,
                                                Position = new Vector2(-0.5421481f, -0.2287672f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 13f,
                                                Position = new Vector2(-0.4758097f, -0.2273623f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 130,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -41f,
                                                Position = new Vector2(-0.4835994f, -0.2074512f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -2f,
                                                Position = new Vector2(-0.41786f, -0.203935f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 130,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -44f,
                                                Position = new Vector2(-0.7851321f, -0.2384659f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -5f,
                                                Position = new Vector2(-0.7050338f, -0.2381582f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 170,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -49f,
                                                Position = new Vector2(-0.8808565f, -0.2363269f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -9f,
                                                Position = new Vector2(-0.788793f, -0.2338803f),
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
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 75,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -51f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 5,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -56f,
                                                Position = new Vector2(-0.6178045f, -0.1444166f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -9f,
                                                Position = new Vector2(-0.5802653f, -0.1515768f),
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
                                                Position = new Vector2(-0.6178045f, -0.1444166f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -9f,
                                                Position = new Vector2(-0.5802653f, -0.1515768f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 180,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -72f,
                                                Position = new Vector2(-0.8483325f, -0.1163473f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -33f,
                                                Position = new Vector2(-0.7927856f, -0.1288491f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 180,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -76f,
                                                Position = new Vector2(-0.5129914f, -0.1302504f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -26f,
                                                Position = new Vector2(-0.4574445f, -0.1542533f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                },
                            },
                            new VertexAnimation( ) {
                                Name = "attack0",
                                Frames = new VertexAnimationFrame[ ] {
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -1f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 200,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -46f,
                                                Position = new Vector2(0.236952f, -0.1529724f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 4f,
                                                Position = new Vector2(0.3750397f, -0.1547852f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 1,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                },
                            },
                            new VertexAnimation( ) {
                                Name = "attack1",
                                Frames = new VertexAnimationFrame[ ] {
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -17f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -48f,
                                                Position = new Vector2(0.1655701f, -0.1572503f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 3f,
                                                Position = new Vector2(0.2871601f, -0.1579936f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 70,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -48f,
                                                Position = new Vector2(-0.3077279f, -0.1251661f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 3f,
                                                Position = new Vector2(-0.1693201f, -0.1344652f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 200,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -43f,
                                                Position = new Vector2(0.5187417f, -0.1444166f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 3f,
                                                Position = new Vector2(0.5610483f, -0.143021f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 1,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -1f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                },
                            },
                            new VertexAnimation( ) {
                                Name = "attack2",
                                Frames = new VertexAnimationFrame[ ] {
                                    new VertexAnimationFrame( ) {
                                        Time = 150,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -1f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -53f,
                                                Position = new Vector2(-0.3125329f, 0.245941f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 3f,
                                                Position = new Vector2(-0.1308797f, 0.2334335f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 150,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -55f,
                                                Position = new Vector2(0.7445793f, -0.0663451f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -16f,
                                                Position = new Vector2(0.8301314f, -0.05746315f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 1,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "rapier"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -1f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                },
                            },
                    }
                },
                new Transform(new Vector2(1.5f / 2.6f * owner.Transform.Width, -1.4f / 6.2f * owner.Transform.Height), new Vector2(7f / 2.6f * owner.Transform.Width, 1f / 6.3f * owner.Transform.Height)),
                owner);
        }
    }
}

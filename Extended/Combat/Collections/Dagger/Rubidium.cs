using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.Animation;

namespace mapKnight.Extended.Combat.Collections {
    public static partial class Dagger {
        public static PrimaryWeapon Rubidium (Entity owner) {
            return new PrimaryWeapon(
                "Rubidium Dagger", 3, .7f, 50, "weapons/dagger/rubidium", 90,
                new VertexAnimationData( ) {
                    Offsets = new Vector2[ ] {
                            new Vector2(1, 13),  // dagger
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
                                                Position = new Vector2(-0.8450318f, -0.2522976f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.784857f, -0.254059f),
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                                Texture = "dagger"
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
                                        Time = 90,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 120,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -42f,
                                                Position = new Vector2(0.1312408f, -0.1968208f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -2f,
                                                Position = new Vector2(0.2597184f, -0.1954252f),
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
                                                Texture = "dagger"
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
                                        Time = 150,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -4f,
                                                Position = new Vector2(-0.6089827f, -0.2382041f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 80,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = 12f,
                                                Position = new Vector2(0.2348088f, 0.1376692f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 45f,
                                                Position = new Vector2(0.2948723f, 0.1926367f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -59f,
                                                Position = new Vector2(0.4256599f, -0.2214383f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -16f,
                                                Position = new Vector2(0.588935f, -0.2326017f),
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
                                                Texture = "dagger"
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
                                Name = "attack2",
                                Frames = new VertexAnimationFrame[ ] {
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -47f,
                                                Position = new Vector2(-0.6560944f, -0.2385302f),
                                                Texture = "dagger"
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
                                                Rotation = -51f,
                                                Position = new Vector2(0.1535574f, -0.167945f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -11f,
                                                Position = new Vector2(0.27755f, -0.1857999f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -49f,
                                                Position = new Vector2(-0.449477f, -0.09842929f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = -11f,
                                                Position = new Vector2(-0.332692f, -0.1087979f),
                                                Texture = "hand1"
                                            },
                                        }
                                    },
                                    new VertexAnimationFrame( ) {
                                        Time = 100,
                                        State = new VertexBone[ ] {
                                            new VertexBone( ) {
                                                Rotation = -37f,
                                                Position = new Vector2(0.7854222f, -0.02356619f),
                                                Texture = "dagger"
                                            },
                                            new VertexBone( ) {
                                                Rotation = 10f,
                                                Position = new Vector2(0.8541566f, -0.02858739f),
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
                                                Texture = "dagger"
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
                    }
                },
                new Transform(new Vector2(.5f / 3.2f * owner.Transform.Width, -1.2f / 6f * owner.Transform.Height), new Vector2(4f / 3.2f * owner.Transform.Width, 3f / 23f * owner.Transform.Height)),
                owner);
        }
    }
}

using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.AI.Guardian;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Player;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics.Animation;

// ( ͡° ͜ʖ ͡°)

/* FORMULA FOR THE BONE RECTANGLES
 * X = (TOP_LEFT_CORNER_X - IMAGE_WIDTH / 2) / IMAGE_WIDTH
 * Y = (TOP_LEFT_CORNER_X - IMAGE_HEIGHT / 2) / IMAGE_HEIGHT 
 * W = IMAGE_WIDTH / BOX_WIDTH
 * H = IMAGE_HEIHT / BOX_HEIGHT
 */

namespace mapKnight.Extended {
    public static class EntityCollection {
        private const int MAX_TIME = int.MaxValue - 86400001;

        public static class Enemys {
            public static class Guardians {
                private static Entity.Configuration _Tent;
                public static Entity.Configuration Tent {
                    get {
                        if (_Tent == null) {
                            _Tent = new Entity.Configuration( );
                            _Tent.Name = "Testing Tent";
                            _Tent.Transform = new Transform(Vector2.Zero, new Vector2(3.405405405f, 2f));
                            _Tent.Components = new ComponentList {
                            new TentComponent.Configuration( ) {
                                Officer = Officer1, PatrolRange = 14f, PrivateCount = 4, TimeBetweenPrivates = 3000,
                                Privates = new Entity.Configuration[ ] { Private1, Private2 }
                            },
                            new TextureComponent.Configuration( ) {
                                Texture = "guardian/guardian",
                                Sprites = new string[ ] {
                                    "tent"
                                }
                            },
                            new DrawComponent.Configuration( ),
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } }
                        };
                        }
                        return _Tent;
                    }
                }

                private static Entity.Configuration _Private1;
                public static Entity.Configuration Private1 {
                    get {
                        if (_Private1 == null) {
                            _Private1 = new Entity.Configuration( );
                            _Private1.Name = "Private1";
                            _Private1.Transform = new Transform(Vector2.Zero, new Vector2(1.5f / 19f * 11f, 1.5f));
                            _Private1.Components = new ComponentList {
                                new MotionComponent.Configuration( ),
                                new PrivateComponent.Configuration( ) { AttackCooldown = 777, Damage = 0.4f },
                                new SpriteComponent.Configuration( ) {
                                    Texture = "guardian/guardian" ,
                                    Animations = new SpriteAnimation[ ] {
                                        new SpriteAnimation( ) {
                                            Name = "walk",
                                            CanRepeat = true,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "1walk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "1idle"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "1walk2"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "1idle"
                                                    }
                                                },
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "atk",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 50,
                                                    Bones = new string[ ] {
                                                        "1idle"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 50,
                                                    Bones = new string[ ] {
                                                        "1atk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 50,
                                                    Bones = new string[ ] {
                                                        "1atk2"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "def",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "1def"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "hurt",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 130,
                                                    Bones = new string[ ] {
                                                        "1hurt"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(9.5f / 23f , 0f, 23f / 11f, 1f) } },
                                new SpeedComponent.Configuration( ) { X = 1.2f },
                                new HealthComponent.Configuration( ) { Value = 5 }
                            };
                        }
                        return _Private1;
                    }
                }

                private static Entity.Configuration _Private2;
                public static Entity.Configuration Private2 {
                    get {
                        if (_Private2 == null) {
                            _Private2 = new Entity.Configuration( );
                            _Private2.Name = "Private2";
                            _Private2.Transform = new Transform(Vector2.Zero, new Vector2(1.5f / 19f * 11f, 1.5f));
                            _Private2.Components = new ComponentList {
                                new MotionComponent.Configuration( ),
                                new PrivateComponent.Configuration( ) { AttackCooldown = 1000, Damage = 1f },
                                new SpriteComponent.Configuration( ) {
                                    Texture = "guardian/guardian",
                                    Animations = new SpriteAnimation[ ] {
                                        new SpriteAnimation( ) {
                                            Name = "walk",
                                            CanRepeat = true,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "2walk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "2idle"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "2walk2"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 70,
                                                    Bones = new string[ ] {
                                                        "2idle"
                                                    }
                                                },
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "atk",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "2idle"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "2atk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "2atk2"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "def",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "2def"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "hurt",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 130,
                                                    Bones = new string[ ] {
                                                        "2hurt"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(15f / 35f, 0f, 35f / 11f, 1f) } },
                                new SpeedComponent.Configuration( ) { X = 1.2f },
                                new HealthComponent.Configuration( ) { Value = 5 }
                            };
                        }
                        return _Private2;
                    }
                }

                private static Entity.Configuration _Officer1;
                public static Entity.Configuration Officer1 {
                    get {
                        if (_Officer1 == null) {
                            _Officer1 = new Entity.Configuration( );
                            _Officer1.Name = "Officer1";
                            _Officer1.Transform = new Transform(Vector2.Zero, new Vector2(1f, 19f / 12f));
                            _Officer1.Components = new ComponentList {
                                new MotionComponent.Configuration( ),
                                new OfficerComponent.Configuration( ) { TurnTime = 1500, AttackTime = 1000, Damage = Mathf.PI / 6f },
                                new SpriteComponent.Configuration( ) {
                                    Texture = "guardian/guardian",
                                    Animations = new SpriteAnimation[ ] {
                                        new SpriteAnimation( ) {
                                            Name = "idle",
                                            CanRepeat = true,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = MAX_TIME,
                                                    Bones = new string[ ] {
                                                        "oidle"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "walk",
                                            CanRepeat = true,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 80,
                                                    Bones = new string[ ] {
                                                        "owalk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 80,
                                                    Bones = new string[ ] {
                                                        "owalk2"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "atk",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "oidle"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "oatk1"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "oatk2"
                                                    }
                                                },
                                                new SpriteAnimationFrame( ) {
                                                    Time = 100,
                                                    Bones = new string[ ] {
                                                        "oatk3"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "def",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time  = 10000,
                                                    Bones = new string[ ] {
                                                        "odef"
                                                    }
                                                }
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "hurt",
                                            CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) {
                                                    Time  = 130,
                                                    Bones = new string[ ] {
                                                        "ohurt"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(9f / 25f, 0.5f / 21f, 25f / 10f, 21f / 20f) } },
                                new SpeedComponent.Configuration( ) { X = 2.4f },
                                new HealthComponent.Configuration( ) { Value = 3 }
                            };
                        }
                        return _Officer1;
                    }
                }
            }

            private static Entity.Configuration _Slime;
            public static Entity.Configuration Slime {
                get {
                    if (_Slime == null) {
                        _Slime = new Entity.Configuration( );
                        _Slime.Name = "Slime";
                        _Slime.Transform = new Transform(Vector2.Zero, new Vector2(1f, 0.95238095f));
                        _Slime.Components = new ComponentList {
                            new SpriteComponent.Configuration( ) {
                                Texture = "slime", Animations = new[ ] {
                                    new SpriteAnimation( ) {
                                        CanRepeat = true, Name ="wobble",
                                        Frames = new[ ] {
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "high" }, Time = 150 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "mid" }, Time = 150 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "low" }, Time = 150 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "mid" }, Time = 150 },
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        CanRepeat = true, Name ="rage",
                                        Frames = new[ ] {
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "high" }, Time = 100 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "mid" }, Time = 100 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "low" }, Time = 100 },
                                            new SpriteAnimationFrame(){ Bones=new[ ] { "mid" }, Time = 100 },
                                        }
                                    }
                                }
                            },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Position = Vector2.Zero, Size = Vector2.One } } },
                            new SlimeComponent.Configuration( ) { SlimeConfig = _Slime, SizeMultiplier = 0.8f, SplitCount = 3, RageRange = 6, RageSpeedMultiplier = 3 },
                            new HealthComponent.Configuration( ) { Value = 1 },
                            new SpeedComponent.Configuration( ) { X = 2f, Y = 2f }
                        };
                    }
                    return _Slime;
                }
            }

            private static Entity.Configuration _Turret;
            public static Entity.Configuration Turret {
                get {
                    if (_Turret == null) {
                        _Turret = new Entity.Configuration( );
                        _Turret.Name = "Turret";
                        _Turret.Transform = new Transform(Vector2.Zero, new Vector2(1.2f, 0.85161f));
                        _Turret.Components = new ComponentList {
                            new TextureComponent.Configuration( ) { Texture = "turret" },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                            new TurretComponent.Configuration( ) {
                                BulletSpawnpointYPercent = 0.71f,
                                LockOnTargetTime = 3000,
                                TimeBetweenShots = 333,
                                TimeBetweenTurns = 5000,
                                BulletSpeed = 7,
                                Bullet = new Entity.Configuration( ) {
                                    Name = "Turret Bullet",
                                    Transform = new Transform(Vector2.Zero, new Vector2(0.4f, 0.4f)),
                                    Components = new ComponentList( ) {
                                        new MotionComponent.Configuration( ) { GravityInfluence = 0 },
                                        new BulletComponent.Configuration( ) { Damage = 0.5f },
                                        new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                                        new TextureComponent.Configuration( ) { Texture = "turret_bullet" }
                                    }
                                }
                            },
                            new TriggerComponent.Configuration( ) {
                                TriggerZone = new Vector2(16, 0.4f),
                                Offset = 0.365f
                            },
                            new HealthComponent.Configuration( ) { Value = 10 }
                        };
                    }
                    return _Turret;
                }
            }

            private static Entity.Configuration _Plugger;
            public static Entity.Configuration Plugger {
                get {
                    if (_Plugger == null) {
                        _Plugger = new Entity.Configuration( );
                        _Plugger.Name = "Plugger";
                        _Plugger.Transform = new Transform(Vector2.Zero, new Vector2(1.35f, 1f));
                        _Plugger.Components = new ComponentList( );
                        _Plugger.Components.Add(new SpriteComponent.Configuration( ) {
                            Texture = "plugger",
                            Animations = new SpriteAnimation[ ] {
                                new SpriteAnimation( ) {
                                    Name = "walk",
                                    CanRepeat = true,
                                    Frames = new SpriteAnimationFrame[ ] {
                                        new SpriteAnimationFrame( ) {
                                            Time = 200,
                                            Bones = new string[ ] {
                                                "walk0"
                                            }
                                        },
                                        new SpriteAnimationFrame( ) {
                                            Time = 200,
                                            Bones = new string[ ] {
                                                "walk1"
                                            }
                                        }
                                    }
                                },
                                new SpriteAnimation( ) {
                                    Name = "shot",
                                    CanRepeat = false,
                                    Frames = new SpriteAnimationFrame[ ] {
                                        new SpriteAnimationFrame( ) {
                                            Time = 100,
                                            Bones = new string[ ] {
                                                "shot0"
                                            }
                                        },
                                        new SpriteAnimationFrame( ) {
                                            Time = 200,
                                            Bones = new string[ ] {
                                                "shot1"
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        _Plugger.Components.Add(new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } });
                        _Plugger.Components.Add(new PluggerComponent.Configuration( ) {
                            TimeBetweenThrows = 4000,
                            BulletSpeed = 1.5f,
                            Bullet = new Entity.Configuration( ) {
                                Name = "Pluggullet",
                                Transform = new Transform(Vector2.Zero, new Vector2(0.4f, 0.4f)),
                                Components = new ComponentList( ) {
                                    new MotionComponent.Configuration( ) {
                                        GravityInfluence = 0.1f
                                    },
                                    new ReturnableBulletComponent.Configuration( ) { ReturnSpeed = 4f, Damage = .1f },
                                    new TextureComponent.Configuration( ) { Texture = "plugger_bullet" },
                                    new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } }
                                }
                            }
                        });
                        _Plugger.Components.Add(new TriggerComponent.Configuration( ) { TriggerZone = new Vector2(8, 3), Offset = 1.5f });
                        _Plugger.Components.Add(new SpeedComponent.Configuration( ) { X = 0.75f });
                        _Plugger.Components.Add(new HealthComponent.Configuration( ) { Value = 3 });
                    }
                    return _Plugger;
                }
            }

            private static Entity.Configuration _Sepling;
            public static Entity.Configuration Sepling {
                get {
                    if (_Sepling == null) {
                        _Sepling = new Entity.Configuration( );
                        _Sepling.Name = "Sepling";
                        _Sepling.Transform = new Transform(Vector2.Zero, new Vector2(1f, 15f / 17f));
                        _Sepling.Components = new ComponentList {
                            new SpriteComponent.Configuration( ) {
                                Texture = "sepling/body",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 300,
                                                Bones = new string[ ] {
                                                    "sepler"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "run",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk2"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "walk",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "walk2"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "def",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 1000,
                                                Bones = new string[ ] {
                                                    "def"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "grow",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 333,
                                                Bones = new string[ ] {
                                                    "grow1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 333,
                                                Bones = new string[ ] {
                                                    "grow2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 333,
                                                Bones = new string[ ] {
                                                    "grow3"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "shoot_prep",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 90,
                                                Bones = new string[ ] {
                                                    "shot0"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 90,
                                                Bones = new string[ ] {
                                                    "shot1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 90,
                                                Bones = new string[ ] {
                                                    "shot2"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "shoot_end",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 90,
                                                Bones = new string[ ] {
                                                    "shot3"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(-2.5f / 36f, 14f / 29f, 36f / 17f, 29f / 15f) } },
                            new SeplingComponent.Configuration( ) {
                                ChillDistance = 5,
                                ShootCooldown = 2000,
                                BulletSpeed = 3,
                                BulletOffset = new Vector2(0.4305f, 0.3793f),
                                Bullet = new Entity.Configuration( ) {
                                    Name = "Sepling Bullet",
                                    Transform = new Transform(Vector2.Zero, new Vector2(0.3f, 0.3f)),
                                    Components = new ComponentList( ) {
                                        new TextureComponent.Configuration( ) {
                                            Texture = "sepling/bullet"
                                        },
                                        new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                                        new BulletComponent.Configuration( ),
                                        new MotionComponent.Configuration( ) { GravityInfluence = 0.3f }
                                    }
                                }
                            },
                            new TriggerComponent.Configuration( ) { TriggerZone = new Vector2(6, 6), Offset = 3 },
                            new SpeedComponent.Configuration( ) { X = 3 },
                            new HealthComponent.Configuration(){ Value = 5 }
                        };
                    }
                    return _Sepling;
                }
            }

            private static Entity.Configuration _Shark;
            public static Entity.Configuration Shark {
                get {
                    if (_Shark == null) {
                        _Shark = new Entity.Configuration( );
                        _Shark.Name = "Shark";
                        _Shark.Transform = new Transform(Vector2.Zero, new Vector2(0.540540f, 1.25f));
                        _Shark.Components = new ComponentList {
                            new MotionComponent.Configuration( ) { GravityInfluence = 0.9f },
                            new SpriteComponent.Configuration( ) {
                                Texture = "shark/body",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "fly",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = MAX_TIME,
                                                Bones = new string[ ] { "jump3" }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "bounce",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump3"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump0"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 30,
                                                Bones = new string[ ] {
                                                    "jump3"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1.8125f, 1) } },
                            new SharkComponent.Configuration( ) { EscapeDistance = 10 },
                            new SpeedComponent.Configuration( ) { X = 5, Y = 20 },
                            new TriggerComponent.Configuration( ) { TriggerZone = new Vector2(16, 10) }
                        };
                    }
                    return _Shark;
                }
            }

            private static Entity.Configuration _Shell;
            public static Entity.Configuration Shell {
                get {
                    if (_Shell == null) {
                        _Shell = new Entity.Configuration( );
                        _Shell.Name = "Shell";
                        _Shell.Transform = new Transform(Vector2.Zero, new Vector2(0.75f, 0.875f));
                        _Shell.Components = new ComponentList {
                            new SpeedComponent.Configuration( ) { X = 1 },
                            new AnimationComponent.Configuration( ) {
                                Textures = new string[ ] {
                                    "shell"
                                },
                                Offsets = new Vector2[ ] {
                                    new Vector2(9.5f, 12.5f)
                                },
                                Scales = new float[ ] {
                                    0.055555555555555555555555f
                                },
                                Animations = new VertexAnimation[ ] {
                                    new VertexAnimation( ) {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 444,
                                                State = new VertexBone[ ] {
                                                    new VertexBone(){
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new VertexAnimation( ) {
                                        Name = "walk",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w1",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w2",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w1",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w2",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w1",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w2",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w1",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w2",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w1",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "w2",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new VertexAnimation( ) {
                                        Name = "prepare",
                                        CanRepeat = false,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "shell",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "def",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new VertexAnimation( ) {
                                        Name = "attack",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 1,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = 0,
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 420,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = 720,
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new VertexAnimation( ) {
                                        Name = "frenzy",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 1440,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = 360,
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 460,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = 180,
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new TriggerComponent.Configuration( ) { TriggerZone = new Vector2(18, 0.5f) },
                            new ShellComponent.Configuration( ) { FrenzySpeed = 0.4f, AttackSpeed = 6.35f },
                            new MotionComponent.Configuration( ),
                            new HealthComponent.Configuration( ) {
                                Value = 3
                            }
                        };
                    }
                    return _Shell;
                }
            }

            private static Entity.Configuration _BlackHole;
            public static Entity.Configuration BlackHole {
                get {
                    if (_BlackHole == null) {
                        _BlackHole = new Entity.Configuration( );
                        _BlackHole.Name = "Black Hole";
                        _BlackHole.Transform = new Transform(Vector2.Zero, new Vector2(3.14159f, 3.14159f));
                        _BlackHole.Components = new ComponentList( );
                        _BlackHole.Components.Add(new BlackHoleComponent.Configuration( ));
                        _BlackHole.Components.Add(new AnimationComponent.Configuration( ) {
                            Textures = new string[ ] {
                                "blackhole"
                            },
                            Offsets = new Vector2[ ] {
                                new Vector2(12, 12),
                                new Vector2(12, 12)
                            },
                            Scales = new float[ ] {
                                0.0416666666f,
                                0.0416666666f
                            },
                            Animations = new VertexAnimation[ ] {
                                new VertexAnimation( ) {
                                    Name = "drot",
                                    CanRepeat = true,
                                    Frames = new VertexAnimationFrame[ ] {
                                        new VertexAnimationFrame( ) {
                                            Time = 9000,
                                            State = new VertexBone[ ] {
                                                new VertexBone( ) {
                                                    Texture = "i",
                                                    Rotation = 0
                                                },
                                                new VertexBone( ) {
                                                    Texture = "o",
                                                    Rotation = 0
                                                }
                                            }
                                        },
                                        new VertexAnimationFrame( ) {
                                            Time = 1,
                                            State = new VertexBone[ ] {
                                                new VertexBone( ) {
                                                    Texture = "i",
                                                    Rotation = 360
                                                },
                                                new VertexBone( ) {
                                                    Texture = "o",
                                                    Rotation = -360
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                    return _BlackHole;
                }
            }
        }

        public static class NPCs {
            private static Entity.Configuration _Lenny;
            public static Entity.Configuration Lenny {
                get {
                    if (_Lenny == null) {
                        _Lenny = new Entity.Configuration( );
                        _Lenny.Name = "Lenny";
                        _Lenny.Transform = new Transform(Vector2.Zero, new Vector2(5f, 1.1f));
                        _Lenny.Components = new ComponentList {
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 0.22f, 1f), new Rectangle(0, 0.9f, 0.075625f, 0.65625f) } },
                            new SpriteComponent.Configuration( ) {
                                Texture = "npc/lenny",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "idle_inactive",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = MAX_TIME,
                                                Bones= new string[ ] {
                                                    "idle",
                                                    "inactive"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "idle_active",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones= new string[ ] {
                                                    "idle",
                                                    "active"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "idle_none",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = MAX_TIME,
                                                Bones= new string[ ] {
                                                    "idle",
                                                    "none"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "talk",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "talk",
                                                    "none"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "idle",
                                                    "none"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "talk",
                                                    "none"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "deny",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "deny1",
                                                    "none"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "deny2",
                                                    "none"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "deny1",
                                                    "none"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new NPCComponent.Configuration( ) { }
                        };
                    }
                    return _Lenny;
                }
            }
        }

        public static class Obstacles {
            private static Entity.Configuration _CircularSaw;
            public static Entity.Configuration CircularSaw {
                get {
                    if (_CircularSaw == null) {
                        _CircularSaw = new Entity.Configuration( );
                        _CircularSaw.Name = "Circular Saw";
                        _CircularSaw.Transform = new Transform(Vector2.Zero, new Vector2(2f, 2f));
                        _CircularSaw.Components = new ComponentList {
                            new AnimationComponent.Configuration( ) {
                                Textures = new string[ ] {
                                    "saw"
                                },
                                Offsets = new Vector2[ ] {
                                    new Vector2(12, 12)
                                },
                                Scales = new float[ ] {
                                    0.083333333333f
                                },
                                Animations = new VertexAnimation[ ] {
                                    new VertexAnimation( ) {
                                        Name = "drot",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                 Time = 800,
                                                 State = new VertexBone[ ] {
                                                     new VertexBone( ) {
                                                           Texture = "body",
                                                           Rotation = 0
                                                     }
                                                 }
                                            },
                                            new VertexAnimationFrame( ) {
                                                 Time = 1,
                                                 State = new VertexBone[ ] {
                                                     new VertexBone( ) {
                                                           Texture = "body",
                                                           Rotation = 360
                                                     }
                                                 }
                                            }
                                        }
                                    }
                                }
                            },
                            new CircularSawComponent.Configuration( ) {
                                Speed = 3,
                            }
                        };
                    }
                    return _CircularSaw;
                }
            }

            private static Entity.Configuration _MoonballTriggerButton;
            private static Entity.Configuration _Moonball;
            public static Entity.Configuration Moonball {
                get {
                    if (_Moonball == null) {
                        _Moonball = new Entity.Configuration( );
                        _Moonball.Name = "Moonball";
                        _Moonball.Transform = new Transform(Vector2.Zero, new Vector2(2, 2));
                        _Moonball.Components = new ComponentList( );
                        _Moonball.Components.Add(new AnimationComponent.Configuration( ) {
                            Textures = new string[ ] {
                                "moonball"
                            },
                            Offsets = new Vector2[ ] {
                                new Vector2(25, 25)
                            },
                            Scales = new float[ ] {
                                 0.02f
                            },
                            Animations = new VertexAnimation[ ] {
                                 new VertexAnimation( ) {
                                     Name = "drot",
                                     CanRepeat = true,
                                     Frames = new VertexAnimationFrame[ ] {
                                        new VertexAnimationFrame( ) {
                                            Time = 3000,
                                            State = new VertexBone[ ] {
                                                new VertexBone( ) {
                                                    Position = Vector2.Zero,
                                                    Texture = "body",
                                                    Rotation = 360
                                                }
                                            }
                                        },
                                        new VertexAnimationFrame( ) {
                                            Time = 1,
                                            State = new VertexBone[ ] {
                                                new VertexBone( ) {
                                                    Position = Vector2.Zero,
                                                    Texture = "body",
                                                    Rotation = 0
                                                }
                                            }
                                        }
                                     }
                                 }
                             }
                        });
                        _Moonball.Components.Add(new MoonballComponent.Configuration( ) {
                            BoostVelocity = 2.5f,
                            DamagePerSecond = 4f
                        });

                        _MoonballTriggerButton = new Entity.Configuration( );
                        _MoonballTriggerButton.Name = "Moonball Trigger";
                        _MoonballTriggerButton.Transform = new Transform(Vector2.Zero, new Vector2(1, 0.538461538f));
                        _MoonballTriggerButton.Components = new ComponentList( );
                        _MoonballTriggerButton.Components.Add(new SpriteComponent.Configuration( ) {
                            Texture = "button",
                            Animations = new SpriteAnimation[ ] {
                                new SpriteAnimation( ) {
                                    Name = "up",
                                    CanRepeat = true,
                                    Frames = new SpriteAnimationFrame[ ] {
                                        new SpriteAnimationFrame( ) {
                                            Time = MAX_TIME,
                                            Bones = new string[ ] { "up" }
                                        }
                                    }
                                },
                                new SpriteAnimation( ) {
                                    Name = "down",
                                    CanRepeat = true,
                                    Frames = new SpriteAnimationFrame[ ] {
                                        new SpriteAnimationFrame( ) {
                                            Time = MAX_TIME,
                                            Bones = new string[ ] { "down" }
                                        }
                                    }
                                }
                            }
                        });
                        _MoonballTriggerButton.Components.Add(new MoonballButtonComponent.Configuration( ));
                        _MoonballTriggerButton.Components.Add(new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } });
                    }

                    return _MoonballTriggerButton;
                }
            }

            private static Entity.Configuration _Landmine;
            public static Entity.Configuration Landmine {
                get {
                    if (_Landmine == null) {
                        _Landmine = new Entity.Configuration( );
                        _Landmine.Name = "Landmine";
                        _Landmine.Transform = new Transform(Vector2.Zero, new Vector2(0.7f, 0.21875f));
                        _Landmine.Components = new ComponentList {
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                            new LandMineComponent.Configuration( ) { ExplosionRadius = 5f, ThrowBackSpeed = 20f, Damage = 5f },
                            new SpriteComponent.Configuration( ) {
                                Texture = "landmine",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "idle",
                                        CanRepeat=true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = MAX_TIME,
                                                Bones = new string[ ] {
                                                    "default"
                                                }
                                            }
                                        }
                                    },
                                    new SpriteAnimation( ) {
                                        Name = "explode",
                                        CanRepeat = false,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 500,
                                                Bones = new string[ ] {
                                                    "explode"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 400,
                                                Bones = new string[ ] {
                                                    "default"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 300,
                                                Bones = new string[ ] {
                                                    "explode"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "default"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 100,
                                                Bones = new string[ ] {
                                                    "explode"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    return _Landmine;
                }
            }

            private static Entity.Configuration _Drillbomb;
            public static Entity.Configuration Drillbomb {
                get {
                    if(_Drillbomb == null) {
                        _Drillbomb = new Entity.Configuration("Drillbomb", new Vector2(19f / 29f, 1f)) {
                            Components = new ComponentList {
                                new DrillComponent.Configuration( ),
                                new SpriteComponent.Configuration( ) {
                                    Texture = "drill",
                                    Animations = new SpriteAnimation[ ] {
                                        new SpriteAnimation( ) {
                                            Name = "idle", CanRepeat = true,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "d0" }, Time = 100 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "d1" }, Time = 100 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "d2" }, Time = 100 },
                                            }
                                        },
                                        new SpriteAnimation( ) {
                                            Name = "explode", CanRepeat = false,
                                            Frames = new SpriteAnimationFrame[ ] {
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "d2" }, Time=80 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "ex" }, Time=60 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "ex" }, Time=20 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "d2" }, Time=20 },
                                                new SpriteAnimationFrame( ) { Bones = new string[ ] { "ex" }, Time=20 },
                                            }
                                        }
                                    }
                                },
                                new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                                new TriggerComponent.Configuration( ) { Offset = 0, TriggerZone = new Vector2(3, 3) }
                            }
                        };
                    }
                    return _Drillbomb;
                }
            }
        }

        public static class Platforms {
            private static Entity.Configuration _Copper;
            public static Entity.Configuration Copper {
                get {
                    if (_Copper == null) {
                        _Copper = new Entity.Configuration( );
                        _Copper.Name = "Copper Platform";
                        _Copper.Transform = new Transform(Vector2.Zero, new Vector2(1.5f, 1f));
                        _Copper.Components = new ComponentList {
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
                            new TextureComponent.Configuration( ) { Texture = "platforms/copper" },
                            new PlatformComponent.Configuration( ) { Speed = 2 }
                        };
                    }
                    return _Copper;
                }
            }
        }

        public static class Players {
            private static Entity.Configuration _Diamond;
            public static Entity.Configuration Diamond {
                get {
                    if (_Diamond == null) {
                        _Diamond = new Entity.Configuration( );
                        _Diamond.Name = "Player";
                        _Diamond.Transform = new Transform(Vector2.Zero, new Vector2(1.32f*0.44750841750841724f, 1.32f));
                        _Diamond.Components = new ComponentList {
                            new MotionComponent.Configuration( ) { PlatformCollider = true },
                            new PlayerComponent.Configuration( ) {
                                Health = 10,
                                BodyAnimationData = new VertexAnimationData( ) {
                                    Offsets = new Vector2[ ] {
                                        new Vector2(3, 2.5f),   // feet2
                                        new Vector2(2, 2),      // hand1
                                        new Vector2(3, 2.5f),   // feet1
                                        new Vector2(2, 1.5f),   // upper_arm1
                                        new Vector2(9, 5),      // body
                                        new Vector2(9, 9),      // head
                                        new Vector2(2, 1.5f),   // upper_arm1
                                    },
                                    Scales = new float[ ] {
                                        0.08244598f,
                                        0.08244598f,
                                        0.08244598f,
                                        0.08244598f,
                                        0.08244598f,
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
                                                            Position = new Vector2(0.2558146f, -0.4063475f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6589333f, -0.2468656f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2621972f, -0.4100688f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.5711888f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.09024239f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.1580086f, 0.3166551f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.7219032f, -0.06217242f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 800,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.253838f, -0.4105226f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6572748f, -0.2830519f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2598444f, -0.4100688f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6385429f, -0.07663018f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.04823175f, -0.2011819f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -3f,
                                                            Position = new Vector2(-0.09126966f, 0.2960604f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.6179211f, -0.08374908f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 600,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.253838f, -0.408435f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6612785f, -0.2580262f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4121563f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6265321f, -0.03372891f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.04823175f, -0.1690388f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -3f,
                                                            Position = new Vector2(-0.08680869f, 0.3165276f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.6350563f, -0.03936096f),
                                                            Texture = "upper_arm1"
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
                                                            Position = new Vector2(0.253838f, -0.4063475f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6778371f, -0.2310107f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4058937f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.1066623f, 0.3166551f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.6251004f, -0.04631752f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 100,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = 14f,
                                                            Position = new Vector2(0.4754202f, -0.3076895f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -17f,
                                                            Position = new Vector2(0.5557882f, -0.1860928f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4058937f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -15f,
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 7f,
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -1f,
                                                            Position = new Vector2(-0.1449521f, 0.3027519f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 11f,
                                                            Position = new Vector2(-0.6251004f, -0.04631752f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 130,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = -1f,
                                                            Position = new Vector2(0.4463761f, -0.3999031f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -20f,
                                                            Position = new Vector2(0.528214f, -0.1860836f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4058937f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -21f,
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 7f,
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -4f,
                                                            Position = new Vector2(-0.1234141f, 0.3059604f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 22f,
                                                            Position = new Vector2(-0.6924887f, -0.05491006f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 130,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = -1f,
                                                            Position = new Vector2(0.03560172f, -0.4084957f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 17f,
                                                            Position = new Vector2(0.7364151f, -0.2106815f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -39f,
                                                            Position = new Vector2(-0.01782178f, -0.2891558f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 2f,
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -5f,
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 2f,
                                                            Position = new Vector2(-0.06119308f, 0.3091688f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.6924887f, -0.05491006f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 170,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.1720093f, -0.4084957f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 13f,
                                                            Position = new Vector2(0.7412013f, -0.2106815f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -1f,
                                                            Position = new Vector2(0.08986843f, -0.4078673f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 9f,
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -10f,
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.001365179f, 0.2974046f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -16f,
                                                            Position = new Vector2(-0.6589851f, -0.04849323f),
                                                            Texture = "upper_arm1"
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
                                                            Position = new Vector2(0.253838f, -0.4063475f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6778371f, -0.2310107f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4058937f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6556782f, -0.01387457f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.02799475f, -0.1668116f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.1066623f, 0.3166551f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.6251004f, -0.04631752f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 75,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.253838f, -0.4063475f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6706578f, -0.2491917f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.2645577f, -0.4058937f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.6102089f, -0.04168086f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.06867772f, -0.1967569f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -3f,
                                                            Position = new Vector2(-0.1401659f, 0.2835014f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.658604f, -0.07305434f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 5,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = -4f,
                                                            Position = new Vector2(0.3088797f, -0.4052781f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -4f,
                                                            Position = new Vector2(0.7687755f, -0.1529391f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -17f,
                                                            Position = new Vector2(-0.1784055f, -0.2711402f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.7681546f, 0.0428075f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.07730235f, -0.07590644f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -3f,
                                                            Position = new Vector2(0.09196634f, 0.3562256f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.5676656f, 0.05635188f),
                                                            Texture = "upper_arm1"
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
                                                            Rotation = -4f,
                                                            Position = new Vector2(0.3088797f, -0.4052781f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -4f,
                                                            Position = new Vector2(0.7687755f, -0.1529391f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -17f,
                                                            Position = new Vector2(-0.1784055f, -0.2711402f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.7681546f, 0.0428075f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.07730235f, -0.07590644f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -3f,
                                                            Position = new Vector2(0.09196634f, 0.3562256f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(-0.5676656f, 0.05635188f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 180,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = -4f,
                                                            Position = new Vector2(0.5340359f, -0.4250605f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 16f,
                                                            Position = new Vector2(0.9449321f, -0.126454f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -17f,
                                                            Position = new Vector2(-0.3012263f, -0.3420686f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 20f,
                                                            Position = new Vector2(0.8160169f, 0.05029381f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Position = new Vector2(0.02937227f, -0.1053484f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 10f,
                                                            Position = new Vector2(0.01777975f, 0.3668803f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -24f,
                                                            Position = new Vector2(-0.588635f, 0.05367533f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                                new VertexAnimationFrame( ) {
                                                    Time = 180,
                                                    State = new VertexBone[ ] {
                                                        new VertexBone( ) {
                                                            Rotation = 23f,
                                                            Position = new Vector2(0.4167224f, -0.3597768f),
                                                            Texture = "feet2"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 9f,
                                                            Position = new Vector2(0.8851042f, -0.1692329f),
                                                            Texture = "hand1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 13f,
                                                            Position = new Vector2(-0.3419093f, -0.3728145f),
                                                            Texture = "feet1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -1f,
                                                            Position = new Vector2(0.7681546f, 0.0428075f),
                                                            Texture = "upper_arm1"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -8f,
                                                            Position = new Vector2(0.02937227f, -0.1053484f),
                                                            Texture = "body"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = 6f,
                                                            Position = new Vector2(0.09196634f, 0.318754f),
                                                            Texture = "head"
                                                        },
                                                        new VertexBone( ) {
                                                            Rotation = -5f,
                                                            Position = new Vector2(-0.588635f, 0.05367533f),
                                                            Texture = "upper_arm1"
                                                        },
                                                    }
                                                },
                                            },
                                        },
                                    }
                                }
                            },
                            new SpeedComponent.Configuration( ) { X = 3.5f, Y = 20 },
                            new PlayerAnimationComponent.Configuration( ) { }
                        };
                    }
                    return _Diamond;
                }
            }
        }
    }
}

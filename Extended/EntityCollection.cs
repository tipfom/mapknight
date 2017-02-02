using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.AI.Guardian;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Player;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics.Animation;

// ( ͡° ͜ʖ ͡°)
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
                                Officer = Officer1, PatrolRange = 12f, PrivateCount = 4, TimeBetweenPrivates = 3000,
                                Privates = new Entity.Configuration[ ] { Private1, Private2, Private3 }
                            },
                            new TextureComponent.Configuration( ) { Texture = "guardian/tent" },
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
                            _Private1.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
                            _Private1.Components = new ComponentList {
                            new MotionComponent.Configuration( ),
                            new PrivateComponent.Configuration( ),
                            new TextureComponent.Configuration( ) { Texture = "guardian/private1" },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } },
                            new SpeedComponent.Configuration( ) { X = 1.2f },
                            new HealthComponent.Configuration( ) { Value = 1 }
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
                            _Private2.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
                            _Private2.Components = new ComponentList {
                            new MotionComponent.Configuration( ),
                            new PrivateComponent.Configuration( ),
                            new TextureComponent.Configuration( ) { Texture = "guardian/private2" },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } },
                            new SpeedComponent.Configuration( ) { X = 1.2f },
                            new HealthComponent.Configuration( ) { Value = 1 }
                        };
                        }
                        return _Private2;
                    }
                }

                private static Entity.Configuration _Private3;
                public static Entity.Configuration Private3 {
                    get {
                        if (_Private3 == null) {
                            _Private3 = new Entity.Configuration( );
                            _Private3.Name = "Private3";
                            _Private3.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
                            _Private3.Components = new ComponentList {
                            new MotionComponent.Configuration( ),
                            new PrivateComponent.Configuration( ),
                            new TextureComponent.Configuration( ) { Texture = "guardian/private3" },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } },
                            new SpeedComponent.Configuration( ) { X = 1.2f },
                            new HealthComponent.Configuration( ) { Value = 1 }
                        };
                        }
                        return _Private3;
                    }
                }

                private static Entity.Configuration _Officer1;
                public static Entity.Configuration Officer1 {
                    get {
                        if (_Officer1 == null) {
                            _Officer1 = new Entity.Configuration( );
                            _Officer1.Name = "Officer1";
                            _Officer1.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
                            _Officer1.Components = new ComponentList {
                            new MotionComponent.Configuration( ),
                            new OfficerComponent.Configuration( ),
                            new TextureComponent.Configuration( ) { Texture = "guardian/officer" },
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } },
                            new SpeedComponent.Configuration( ) { X = 1.2f },
                            new HealthComponent.Configuration( ) { Value = 1 }
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
                                    new BulletComponent.Configuration( ),
                                    new TextureComponent.Configuration( ) { Texture = "plugger_bullet" },
                                    new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } }
                                }
                            }
                        });
                        _Plugger.Components.Add(new TriggerComponent.Configuration( ) { TriggerZone = new Vector2(8, 3), Offset = 1.5f });
                        _Plugger.Components.Add(new SpeedComponent.Configuration( ) { X = 0.75f });
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
                        _Sepling.Transform = new Transform(Vector2.Zero, new Vector2(1f, 0.88f));
                        _Sepling.Components = new ComponentList {
                            new SpriteComponent.Configuration( ) {
                                Texture = "sepling/body",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = 500,
                                                Bones = new string[ ] {
                                                    "sepler"
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
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0.0277777777f, 0.2692307692f, 2.1176470588f, 1.7333333333f) } },
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
                            new SpeedComponent.Configuration( ) { X = 3 }
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
                        _Shark.Transform = new Transform(Vector2.Zero, new Vector2(0.97972f, 1.25f));
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
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 1, 1) } },
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
                                Scales = new float[ ] {
                                    0.055555555555555555555555f
                                },
                                Animations = new VertexAnimation[ ] {
                                    new VertexAnimation( ) {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = MAX_TIME,
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
                                        CanRepeat = false,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 420,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = -360,
                                                        Position = new Vector2(0.0277777777f, -0.0714285714f)
                                                    }
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 420,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Texture = "atk",
                                                        Rotation = 360,
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
                            new MotionComponent.Configuration( )
                        };
                    }
                    return _Shell;
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
                            new SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle(0, 0, 0.22f, 1f) } },
                            new SpriteComponent.Configuration( ) {
                                Texture = "npc/lenny",
                                Animations = new SpriteAnimation[ ] {
                                    new SpriteAnimation( ) {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new SpriteAnimationFrame[ ] {
                                            new SpriteAnimationFrame( ) {
                                                Time = MAX_TIME,
                                                Bones= new string[ ] {
                                                    "idle"
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
                                                    "talk"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "idle"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "talk"
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
                                                    "deny1"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "deny2"
                                                }
                                            },
                                            new SpriteAnimationFrame( ) {
                                                Time = 200,
                                                Bones = new string[ ] {
                                                    "deny1"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new NPCComponent.Configuration( ) {
                                Messages = new string[ ] {
                                "This ist ein totaler TEST!",
                                "How are you gehen?",
                                "WHAT DO YOU THINK OF SPAM?\nCACH ME OUSSIDE, HOW BOUT DAH!\nZEILENUMBRUECHE!\nKAPPA123NOKAPPA\n.period",
                                "Wer das liest ist cool"
                            }
                            }
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
                                Waypoints = new Vector2[ ] {
                                    new Vector2(0, 0),
                                    new Vector2(1, 1)
                                }
                            }
                        };
                    }
                    return _CircularSaw;
                }
            }

            private static Entity.Configuration _Moonball;
            public static Entity.Configuration Moonball {
                get {
                    if(_Moonball == null) {
                        _Moonball = new Entity.Configuration( );
                        _Moonball.Name = "Moonball";
                        _Moonball.Transform = new Transform(Vector2.Zero, new Vector2(2, 2));
                        _Moonball.Components.Add(new AnimationComponent.Configuration( ) {
                             Scales = new float[ ] {
                                 0.04f
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
                                                    Texture = "body",
                                                    Rotation = 360
                                                }
                                            }
                                        },
                                        new VertexAnimationFrame( ) {
                                            Time = 1,
                                            State = new VertexBone[ ] {
                                                new VertexBone( ) {
                                                    Texture = "body",
                                                    Rotation = 0
                                                }
                                            }
                                        }
                                     }
                                 }
                             }
                        });
                    }
                    return _Moonball;
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
                            new PlatformComponent.Configuration( ) {
                                Speed = 3,
                                Waypoints = new Vector2[ ] {
                                    new Vector2(0, 0),
                                    new Vector2(6, 8),
                                    new Vector2(0, 6)
                                }
                            }
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
                        _Diamond.Transform = new Transform(Vector2.Zero, new Vector2(1f, 1.32f));
                        _Diamond.Components = new ComponentList {
                            new MotionComponent.Configuration( ) { PlatformCollider = true },
                            new PlayerComponent.Configuration( ) { Weapon = "Swords.WoodySword", Health = 10 },
                            new SpeedComponent.Configuration( ) { X = 3.5f, Y = 20 },
                            new AnimationComponent.Configuration( ) {
                                Scales = new float[ ] {
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f,
                                    0.0487060845f
                                },
                                Animations = new VertexAnimation[ ] {
                                    new VertexAnimation() {
                                        Name = "idle",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 500,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4026417f, -0.2310107f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.060812f, 0.3166551f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3853965f, -0.2385302f),
                                                        Rotation = -47f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 800,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3904943f, -0.2830519f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3794281f, -0.07663018f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.02629337f, -0.2011819f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.0517186f, 0.2960604f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3628451f, -0.08374908f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3896378f, -0.2748923f),
                                                        Rotation = -45f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2895388f),
                                                        Rotation = 2f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 600,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3928595f, -0.2580262f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3723325f, -0.03372891f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.02629337f, -0.1690388f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.04908322f, 0.3165276f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.372968f, -0.03936096f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3933463f, -0.2460565f),
                                                        Rotation = -51f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3644413f, -0.2555707f),
                                                        Rotation = -11f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new VertexAnimation() {
                                        Name = "walk",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4026417f, -0.2310107f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.060812f, 0.3166551f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3938791f, -0.2363913f),
                                                        Rotation = -47f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 150,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3000263f, -0.3440516f),
                                                        Rotation = 14f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3305396f, -0.1860928f),
                                                        Rotation = -17f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Rotation = -15f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Rotation = 7f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.08343226f, 0.3027519f),
                                                        Rotation = -1f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Rotation = 11f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3180811f, -0.2287672f),
                                                        Rotation = -33f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.2788909f, -0.2273623f),
                                                        Rotation = 13f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 150,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.265903f, -0.3999031f),
                                                        Rotation = -1f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3142498f, -0.1860836f),
                                                        Rotation = -20f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Rotation = -21f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Rotation = 7f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.07070836f, 0.3059604f),
                                                        Rotation = -4f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4068969f, -0.05491006f),
                                                        Rotation = 22f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.2834927f, -0.2074512f),
                                                        Rotation = -41f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.2446563f, -0.203935f),
                                                        Rotation = -2f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.0232324f, -0.4084957f),
                                                        Rotation = -1f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4372475f, -0.2106815f),
                                                        Rotation = 17f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.02529345f, -0.3426294f),
                                                        Rotation = -39f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Rotation = 2f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Rotation = -5f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.03395045f, 0.3091688f),
                                                        Rotation = 2f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4068969f, -0.05491006f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4616272f, -0.2384659f),
                                                        Rotation = -44f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.421377f, -0.2381582f),
                                                        Rotation = -5f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 200,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1038171f, -0.4084957f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.440075f, -0.2106815f),
                                                        Rotation = 13f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.0552912f, -0.4078673f),
                                                        Rotation = -1f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Rotation = 9f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Rotation = -10f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.001393698f, 0.2974046f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4068969f, -0.05491006f),
                                                        Rotation = -16f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.5407981f, -0.2395354f),
                                                        Rotation = -49f,
                                                        Texture = "sword"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4892378f, -0.2338803f),
                                                        Rotation = -9f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new VertexAnimation() {
                                        Name = "jump",
                                        CanRepeat = false,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4026417f, -0.2310107f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.060812f, 0.3166551f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3853965f, -0.2385302f),
                                                        Rotation = -47f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3984004f, -0.2491917f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3626894f, -0.04168086f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.03837211f, -0.1967569f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.08060472f, 0.2835014f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3868791f, -0.07305434f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3853965f, -0.2385302f),
                                                        Rotation = -51f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 100,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1846751f, -0.4052781f),
                                                        Rotation = -4f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4563648f, -0.1529391f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1031953f, -0.2711402f),
                                                        Rotation = -17f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.455998f, 0.0428075f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.04786761f, -0.07590644f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.05653057f, 0.3562256f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.333156f, 0.05635188f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3627762f, -0.1444166f),
                                                        Rotation = -56f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3405994f, -0.1515768f),
                                                        Rotation = -9f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new VertexAnimation() {
                                        Name = "hit",
                                        CanRepeat = false,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 300,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4026417f, -0.2310107f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.060812f, 0.3166551f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3853965f, -0.2385302f),
                                                        Rotation = -47f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 130,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1970704f, -0.325648f),
                                                        Rotation = 9f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3464464f, -0.1977958f),
                                                        Rotation = -2f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3327864f, 0.002213408f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.05927676f, -0.1578739f),
                                                        Rotation = 17f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.172874f, 0.3037908f),
                                                        Rotation = 9f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4522333f, -0.07670592f),
                                                        Rotation = 85f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1890052f, -0.1126988f),
                                                        Rotation = 26f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1681619f, -0.07521598f),
                                                        Rotation = 75f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 250,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.245788f, -0.4070656f),
                                                        Rotation = -1f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.417704f, -0.3078755f),
                                                        Rotation = 2f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1422649f, -0.3944962f),
                                                        Rotation = -9f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3800367f, -0.08216827f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.0001469269f, -0.1900498f),
                                                        Rotation = -13f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.03444532f, 0.2637313f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3718168f, -0.01950422f),
                                                        Rotation = 27f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.09572338f, -0.158121f),
                                                        Rotation = -56f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.0404874f, -0.1699104f),
                                                        Rotation = -12f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 1,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1521585f, -0.4063475f),
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4026417f, -0.2310107f),
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1540909f, -0.4058937f),
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.389551f, -0.01387457f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.01433809f, -0.1668116f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.060812f, 0.3166551f),
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3670864f, -0.04631752f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3853965f, -0.2385302f),
                                                        Rotation = -47f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3575646f, -0.2382041f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new VertexAnimation() {
                                        Name = "fall",
                                        CanRepeat = true,
                                        Frames = new VertexAnimationFrame[ ] {
                                            new VertexAnimationFrame( ) {
                                                Time = 200,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.1846751f, -0.4052781f),
                                                        Rotation = -4f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4563648f, -0.1529391f),
                                                        Rotation = -4f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1031953f, -0.2711402f),
                                                        Rotation = -17f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.455998f, 0.0428075f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.04786761f, -0.07590644f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.05653057f, 0.3562256f),
                                                        Rotation = -3f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.333156f, 0.05635188f),
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3627762f, -0.1444166f),
                                                        Rotation = -56f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3405994f, -0.1515768f),
                                                        Rotation = -9f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 200,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.3176892f, -0.4250605f),
                                                        Rotation = -4f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.5604317f, -0.126454f),
                                                        Rotation = 16f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1757534f, -0.3420686f),
                                                        Rotation = -17f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.4842733f, 0.05029381f),
                                                        Rotation = 20f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.01955226f, -0.1053484f),
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.01270382f, 0.3668803f),
                                                        Rotation = 10f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.345544f, 0.05367533f),
                                                        Rotation = -24f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4989638f, -0.1163473f),
                                                        Rotation = -72f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.4661487f, -0.1288491f),
                                                        Rotation = -33f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                            new VertexAnimationFrame( ) {
                                                Time = 200,
                                                State = new VertexBone[ ] {
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.2483846f, -0.3597768f),
                                                        Rotation = 23f,
                                                        Texture = "feet2"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.5250875f, -0.1692329f),
                                                        Rotation = 9f,
                                                        Texture = "hand1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.1997874f, -0.3728145f),
                                                        Rotation = 13f,
                                                        Texture = "feet1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.455998f, 0.0428075f),
                                                        Rotation = -1f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.01955226f, -0.1053484f),
                                                        Rotation = -8f,
                                                        Texture = "body"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(0.05653057f, 0.318754f),
                                                        Rotation = 6f,
                                                        Texture = "head"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.345544f, 0.05367533f),
                                                        Rotation = -5f,
                                                        Texture = "upper_arm1"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.3008564f, -0.1302504f),
                                                        Rotation = -76f,
                                                        Texture = "weapon"
                                                    },
                                                    new VertexBone( ) {
                                                        Position = new Vector2(-0.2680414f, -0.1542533f),
                                                        Rotation = -26f,
                                                        Texture = "hand1"
                                                    },
                                                }
                                            },
                                        }
                                    },
                                }
                            }
                        };
                    }
                    return _Diamond;
                }
            }
        }
    }
}

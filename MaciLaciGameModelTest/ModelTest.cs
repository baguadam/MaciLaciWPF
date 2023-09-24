using Moq;
using ConsoleMaciLaci.Persistence;
using MaciLaci.Model;
using MaciLaci.Persistence;

namespace MaciLaciGameModelTest
{
    [TestClass]
    public class ModelTest
    {
        private Mock<IMaciLaciDataAccess> _mockFileManager = null;
        private MaciLaciModel _model = null;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileManager = new Mock<IMaciLaciDataAccess>();
            Table table = new Table(8);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    table.SetValue(i, j, FieldOptions.FREE);
                }
            }
            table.SetValue(0, 0, FieldOptions.PLAYER);
            table.SetValue(4, 0, FieldOptions.COP);
            table.SetValue(5, 1, FieldOptions.COP);
            table.SetValue(0, 5, FieldOptions.TREE);
            table.SetValue(1, 5, FieldOptions.TREE);
            table.SetValue(2, 5, FieldOptions.TREE);
            table.SetValue(5, 3, FieldOptions.BASKET);
            table.SetValue(5, 4, FieldOptions.BASKET);
            table.SetValue(5, 5, FieldOptions.BASKET);

            _mockFileManager.Setup(mock => mock.Load(It.IsAny<String>())).Returns(() => table);
            _model = new MaciLaciModel(_mockFileManager.Object);
        }

        [TestCleanup]
        public void TestCleanUp()
        {

        }

        private void MockFileLoadTable(Table table)
        {
            _mockFileManager.Setup(mock => mock.Load(It.IsAny<String>())).Returns(() => table);
            _model = new MaciLaciModel(_mockFileManager.Object);
        }

        private Table SetFreeTable()
        {
            Table table = new Table(6);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    table.SetValue(0, 0, FieldOptions.FREE);
                }
            }
            return table;
        }

        [TestMethod]
        public void MaciLaciConstructorTest()
        {
            Table _table = SetFreeTable();
            MockFileLoadTable(_table);

            //a tábla mérete megfelelõen jött lére
            Assert.AreEqual(TableSize.MEDIUM, _model.TableSize);

            //elhelyezõdött a játékos a kezdõpozícióba
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);

            _model.NewGame();
            //új játék esetén is a kezdõpozícióban van
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);

            //rendõrök száma megfelelõ
            Assert.AreEqual(2, _model.CopPosition.Count);
            //fák száma megfelelõ
            Assert.AreEqual(3, _model.TreePosition.Count);
            //kosarak száma megfelelõ a listában
            Assert.AreEqual(3, _model.BasketPosition.Count);
            //begyûjtendõ kosarak száma megfelelõ
            Assert.AreEqual(3, _model.BasketNumber);
        }

        [TestMethod]

        public void MaciLaciStepPlayerTest()
        {
            _model.NewGame();

            //Kilépés a pályáról
            _model.Player.TakeStep(_model.GameTable, Direction.LEFT);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));

            //Kilépés a pályáról
            _model.Player.TakeStep(_model.GameTable, Direction.UP);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));

            //Jobbra lépés
            _model.Player.TakeStep(_model.GameTable, Direction.RIGHT);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(1, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.FREE, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 1));

            //Nemlétezõ mezõ értékének átállítása
            try
            {
                _model.GameTable.SetValue(9, 9, FieldOptions.PLAYER);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }

            //Nemlétezõ mezõ értékének lekérése
            try
            {
                _model.GameTable.GetValue(9, 9);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }

            _model.GameTable = SetFreeTable();
            _model.Player.Position.PositionX = 3;
            _model.Player.Position.PositionY = 4;

            _model.GameTable.SetValue(3, 5, FieldOptions.BASKET);
            _model.Player.TakeStep(_model.GameTable, Direction.RIGHT);

            //Begyûjti-e a kosarat
            Assert.AreEqual(1, _model.Player.CollectedBaskets);

            _model.GameTable.SetValue(5, 3, FieldOptions.COP);
            _model.GameTable.SetValue(1, 1, FieldOptions.COP);
            List<Cop> cops = new List<Cop>();
            cops.Add(new Cop(5, 3));
            cops.Add(new Cop(1, 1));
            _model.CopPosition = cops;
            _model.Player.Position.PositionX = 3;
            _model.Player.Position.PositionY = 3;
            _model.Player.TakeStep(_model.GameTable, Direction.DOWN);

            //Sikerült-e a lépés minden szinten
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(4, 3));

            //Elkapja a rendõr, mert éppen belép
            Assert.IsTrue(_model.CopPosition[0].CopCathes(_model.GameTable));
            //Nem kapja el a rendõr, mert még nem látja a lépés után
            Assert.IsFalse(_model.CopPosition[1].CopCathes(_model.GameTable));

            _model.GameTable.SetValue(1, 1, FieldOptions.TREE);
            _model.GameTable.SetValue(1, 0, FieldOptions.PLAYER);
            _model.Player.Position = new Position(1, 0);
            _model.Player.TakeStep(_model.GameTable, Direction.RIGHT);
            //ha akadályba ütközik, nem történik lépés
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(1, 0));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(1, 1));
            Assert.AreEqual(0, _model.Player.Position.PositionY);

        }

        [TestMethod]
        public void MaciLaciStepCopTest()
        {
            _model.GameTable = SetFreeTable();

            _model.GameTable.SetValue(3, 4, FieldOptions.COP);
            Cop cop = new Cop(3, 4);
            cop.Direction = Direction.RIGHT;
            cop.TakeStep(_model.GameTable);
            //a rendõr megfelelõen lép a megadott pozíció irányába
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(3, 5));

            _model.GameTable.SetValue(4, 5, FieldOptions.TREE);
            cop.Direction = Direction.DOWN;
            cop.TakeStep(_model.GameTable);

            //nem lépett a rendõr az akadály helyére
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(4, 5));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(3, 5));
            //akadályba ütközve megváltozott a haladásának az iránya
            Assert.AreEqual(Direction.UP, cop.Direction);

            _model.GameTable.SetValue(5, 5, FieldOptions.COP);
            cop.Position = new Position(5, 5);
            cop.Direction = Direction.DOWN;
            cop.TakeStep(_model.GameTable);
            //a "falnak" ütközve nem változott a pozíciója
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 5));
            //fentrõl a falnak ütközve felfelé kezd el haladni
            Assert.AreEqual(Direction.UP, cop.Direction);

            cop.Direction = Direction.RIGHT;
            cop.TakeStep(_model.GameTable);
            //a "falnak" ütközve nem változott a pozíciója
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 5));
            //balról a falnak ütközve balra kezd el haladni
            Assert.AreEqual(Direction.LEFT, cop.Direction);

            _model.GameTable.SetValue(0, 0, FieldOptions.COP);
            cop.Position = new Position(0, 0);
            cop.Direction = Direction.UP;
            cop.TakeStep(_model.GameTable);
            //a "falnak" ütközve nem változott a pozíciója
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(0, 0));
            //lentrõl a falnak ütközve lefelé kezd el haladni
            Assert.AreEqual(Direction.DOWN, cop.Direction);

            cop.Direction = Direction.LEFT;
            cop.TakeStep(_model.GameTable);
            //a "falnak" ütközve nem változott a pozíciója
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(0, 0));
            //jobbról a falnak ütközve jobbra kezd el haladni
            Assert.AreEqual(Direction.RIGHT, cop.Direction);

            _model.GameTable.SetValue(3, 3, FieldOptions.COP);
            cop.Position = new Position(3, 3);
            _model.GameTable.SetValue(1, 4, FieldOptions.PLAYER);
            //még nem kapja el a játékost, mert nem látja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 1, FieldOptions.PLAYER);
            //még nem kapja el a játékost, mert nem látja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 5, FieldOptions.PLAYER);
            //még nem kapja el a játékost, mert nem látja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(1, 2, FieldOptions.PLAYER);
            //még nem kapja el a játékost, mert nem látja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));

            _model.GameTable.SetValue(2, 4, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 2, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(2, 2, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 4, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(2, 3, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 3, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 2, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 4, FieldOptions.PLAYER);
            //a rendõr elkapja a játékost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
        }

        [TestMethod]
        [DataRow(-1, 0)]
        [DataRow(9, 3)]
        [DataRow(10, 0)]
        [DataRow(-2, 3)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "X is out of range!")]
        public void MaciLaciSetIndexInvalidTestX(int x, int y)
        {
            _model.NewGame();
            _model.GameTable.SetValue(x, y, FieldOptions.FREE);
        }

        [TestMethod]
        [DataRow(-1, 0)]
        [DataRow(9, 3)]
        [DataRow(10, 0)]
        [DataRow(-2, 3)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "X is out of range!")]
        public void MaciLaciGetIndexInvalidTestX(int x, int y)
        {
            _model.NewGame();
            _model.GameTable.GetValue(x, y);
        }

        [TestMethod]
        [DataRow(0, -1)]
        [DataRow(3, 9)]
        [DataRow(0, 10)]
        [DataRow(2, -2)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Y is out of range!")]
        public void MaciLaciSetIndexInvalidTestY(int x, int y)
        {
            _model.NewGame();
            _model.GameTable.SetValue(x, y, FieldOptions.FREE);
        }

        [TestMethod]
        [DataRow(0, -1)]
        [DataRow(3, 9)]
        [DataRow(0, 10)]
        [DataRow(2, -2)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Y is out of range!")]
        public void MaciLaciGetIndexInvalidTestY(int x, int y)
        {
            _model.NewGame();
            _model.GameTable.GetValue(x, y);
        }

        [TestMethod]
        public void MaciLaciPlayerWins()
        {
            bool eventRaised = false;
            _model.GameOver += delegate (object? sender, MaciLaciEventArgs e)
            {
                eventRaised = true;
                Assert.IsTrue(e.IsWon == true); //beállítódott-e, hogy gyõzött a játékos
            };

            _model.GameTable = SetFreeTable();
            _model.CopPosition = new List<Cop>();
            _model.GameTable.SetValue(0, 2, FieldOptions.BASKET);
            _model.GameTable.SetValue(1, 2, FieldOptions.BASKET);
            _model.GameTable.SetValue(0, 0, FieldOptions.PLAYER);
            _model.Player = new Player(0, 0);
            _model.BasketNumber = 2;
            _model.StepPlayer(Direction.RIGHT);
            _model.StepPlayer(Direction.RIGHT);
            Assert.AreEqual(1, _model.Player.CollectedBaskets);
            _model.StepPlayer(Direction.DOWN);
            Assert.AreEqual(2, _model.Player.CollectedBaskets);

            Assert.IsTrue(eventRaised); //kiváltottuk-e az eseményt
        }

        [TestMethod]
        public void MaciLaciPlayerLoses()
        {
            bool eventRaised = false;
            _model.GameOver += delegate (object? sender, MaciLaciEventArgs e)
            {
                eventRaised = true;
                Assert.IsTrue(e.IsWon == false); //beállítódott-e, hogy veszített a játékos
            };

            _model.GameTable = SetFreeTable();
            _model.GameTable.SetValue(0, 0, FieldOptions.PLAYER);
            _model.Player = new Player(0, 0);
            _model.GameTable.SetValue(3, 1, FieldOptions.COP);
            _model.GameTable.SetValue(3, 1, FieldOptions.COP);
            _model.GameTable.SetValue(5, 5, FieldOptions.BASKET);
            _model.BasketNumber = 1;
            Cop cop1 = new Cop(3, 1);
            cop1.Direction = Direction.UP;
            Cop cop2 = new Cop(2, 4);
            cop2.Direction = Direction.RIGHT;
            List<Cop> cops = new List<Cop>();
            cops.Add(cop1);
            cops.Add(cop2);

            _model.CopPosition = cops;
            _model.StepCops();
            //megtörtént-e a rendõrök léptetése
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 1));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 5));
            _model.StepCops();
            //megtörtént-e a rendõrök léptetése
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(1, 1));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 5));

            //kiváltódott-e az esemény
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void MaciLaciGameLoadTest()
        {
            _model.NewGame();
            _model.StepPlayer(Direction.RIGHT);
            _model.StepPlayer(Direction.LEFT);
            _model.StepCops();

            _model.LoadGame(String.Empty);
            //ellenõrizzük, hogy beállítódtak-e megfelelõen a mezõk
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(4, 0));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 1));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(0, 5));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(1, 5));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(2, 5));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 3));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 4));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 5));

            //ellenõrizzük, hogy a listák rendben vannak-e
            Assert.AreEqual(2, _model.CopPosition.Count);
            Assert.AreEqual(3, _model.TreePosition.Count);
            Assert.AreEqual(3, _model.BasketPosition.Count);

            //ellenõrizzük a kosárszámot
            Assert.AreEqual(3, _model.BasketNumber);

            //ellenõrizzük, hogy meghívták-e a Load mûveletet
            _mockFileManager.Verify(dataAccess => dataAccess.Load(String.Empty), Times.Once());
        }

        [TestMethod]
        public void MaciLaciGameSaveTest()
        {
            _model.NewGame();
            Int32 collectableBaskets = _model.BasketNumber;
            FieldOptions[,] values = new FieldOptions[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    values[i, j] = _model.GameTable.GetValue(i, j);
                }
            }

            //meghívjuk a mentést
            _model.SaveGame(String.Empty);

            //ellenõrizzük, hogy nem változtak-e az értékek mentés után
            Assert.AreEqual(_model.BasketNumber, collectableBaskets);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Assert.AreEqual(values[i, j], _model.GameTable.GetValue(i, j));
                }
            }

            // ellenõrizzük, hogy meghívták-e a Save mûveletet
            _mockFileManager.Verify(mock => mock.Save(String.Empty, It.IsAny<Table>()), Times.Once());
        }
    }
}
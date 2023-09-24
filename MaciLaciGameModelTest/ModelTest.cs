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

            //a t�bla m�rete megfelel�en j�tt l�re
            Assert.AreEqual(TableSize.MEDIUM, _model.TableSize);

            //elhelyez�d�tt a j�t�kos a kezd�poz�ci�ba
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);

            _model.NewGame();
            //�j j�t�k eset�n is a kezd�poz�ci�ban van
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);

            //rend�r�k sz�ma megfelel�
            Assert.AreEqual(2, _model.CopPosition.Count);
            //f�k sz�ma megfelel�
            Assert.AreEqual(3, _model.TreePosition.Count);
            //kosarak sz�ma megfelel� a list�ban
            Assert.AreEqual(3, _model.BasketPosition.Count);
            //begy�jtend� kosarak sz�ma megfelel�
            Assert.AreEqual(3, _model.BasketNumber);
        }

        [TestMethod]

        public void MaciLaciStepPlayerTest()
        {
            _model.NewGame();

            //Kil�p�s a p�ly�r�l
            _model.Player.TakeStep(_model.GameTable, Direction.LEFT);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));

            //Kil�p�s a p�ly�r�l
            _model.Player.TakeStep(_model.GameTable, Direction.UP);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(0, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));

            //Jobbra l�p�s
            _model.Player.TakeStep(_model.GameTable, Direction.RIGHT);
            Assert.AreEqual(0, _model.Player.Position.PositionX);
            Assert.AreEqual(1, _model.Player.Position.PositionY);
            Assert.AreEqual(FieldOptions.FREE, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 1));

            //Neml�tez� mez� �rt�k�nek �t�ll�t�sa
            try
            {
                _model.GameTable.SetValue(9, 9, FieldOptions.PLAYER);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }

            //Neml�tez� mez� �rt�k�nek lek�r�se
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

            //Begy�jti-e a kosarat
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

            //Siker�lt-e a l�p�s minden szinten
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(4, 3));

            //Elkapja a rend�r, mert �ppen bel�p
            Assert.IsTrue(_model.CopPosition[0].CopCathes(_model.GameTable));
            //Nem kapja el a rend�r, mert m�g nem l�tja a l�p�s ut�n
            Assert.IsFalse(_model.CopPosition[1].CopCathes(_model.GameTable));

            _model.GameTable.SetValue(1, 1, FieldOptions.TREE);
            _model.GameTable.SetValue(1, 0, FieldOptions.PLAYER);
            _model.Player.Position = new Position(1, 0);
            _model.Player.TakeStep(_model.GameTable, Direction.RIGHT);
            //ha akad�lyba �tk�zik, nem t�rt�nik l�p�s
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
            //a rend�r megfelel�en l�p a megadott poz�ci� ir�ny�ba
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(3, 5));

            _model.GameTable.SetValue(4, 5, FieldOptions.TREE);
            cop.Direction = Direction.DOWN;
            cop.TakeStep(_model.GameTable);

            //nem l�pett a rend�r az akad�ly hely�re
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(4, 5));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(3, 5));
            //akad�lyba �tk�zve megv�ltozott a halad�s�nak az ir�nya
            Assert.AreEqual(Direction.UP, cop.Direction);

            _model.GameTable.SetValue(5, 5, FieldOptions.COP);
            cop.Position = new Position(5, 5);
            cop.Direction = Direction.DOWN;
            cop.TakeStep(_model.GameTable);
            //a "falnak" �tk�zve nem v�ltozott a poz�ci�ja
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 5));
            //fentr�l a falnak �tk�zve felfel� kezd el haladni
            Assert.AreEqual(Direction.UP, cop.Direction);

            cop.Direction = Direction.RIGHT;
            cop.TakeStep(_model.GameTable);
            //a "falnak" �tk�zve nem v�ltozott a poz�ci�ja
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 5));
            //balr�l a falnak �tk�zve balra kezd el haladni
            Assert.AreEqual(Direction.LEFT, cop.Direction);

            _model.GameTable.SetValue(0, 0, FieldOptions.COP);
            cop.Position = new Position(0, 0);
            cop.Direction = Direction.UP;
            cop.TakeStep(_model.GameTable);
            //a "falnak" �tk�zve nem v�ltozott a poz�ci�ja
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(0, 0));
            //lentr�l a falnak �tk�zve lefel� kezd el haladni
            Assert.AreEqual(Direction.DOWN, cop.Direction);

            cop.Direction = Direction.LEFT;
            cop.TakeStep(_model.GameTable);
            //a "falnak" �tk�zve nem v�ltozott a poz�ci�ja
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(0, 0));
            //jobbr�l a falnak �tk�zve jobbra kezd el haladni
            Assert.AreEqual(Direction.RIGHT, cop.Direction);

            _model.GameTable.SetValue(3, 3, FieldOptions.COP);
            cop.Position = new Position(3, 3);
            _model.GameTable.SetValue(1, 4, FieldOptions.PLAYER);
            //m�g nem kapja el a j�t�kost, mert nem l�tja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 1, FieldOptions.PLAYER);
            //m�g nem kapja el a j�t�kost, mert nem l�tja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 5, FieldOptions.PLAYER);
            //m�g nem kapja el a j�t�kost, mert nem l�tja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(1, 2, FieldOptions.PLAYER);
            //m�g nem kapja el a j�t�kost, mert nem l�tja
            Assert.IsFalse(cop.CopCathes(_model.GameTable));

            _model.GameTable.SetValue(2, 4, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 2, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(2, 2, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 4, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(2, 3, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(4, 3, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 2, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
            Assert.IsTrue(cop.CopCathes(_model.GameTable));
            _model.GameTable.SetValue(3, 4, FieldOptions.PLAYER);
            //a rend�r elkapja a j�t�kost
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
                Assert.IsTrue(e.IsWon == true); //be�ll�t�dott-e, hogy gy�z�tt a j�t�kos
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

            Assert.IsTrue(eventRaised); //kiv�ltottuk-e az esem�nyt
        }

        [TestMethod]
        public void MaciLaciPlayerLoses()
        {
            bool eventRaised = false;
            _model.GameOver += delegate (object? sender, MaciLaciEventArgs e)
            {
                eventRaised = true;
                Assert.IsTrue(e.IsWon == false); //be�ll�t�dott-e, hogy vesz�tett a j�t�kos
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
            //megt�rt�nt-e a rend�r�k l�ptet�se
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 1));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 5));
            _model.StepCops();
            //megt�rt�nt-e a rend�r�k l�ptet�se
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(1, 1));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(2, 5));

            //kiv�lt�dott-e az esem�ny
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
            //ellen�rizz�k, hogy be�ll�t�dtak-e megfelel�en a mez�k
            Assert.AreEqual(FieldOptions.PLAYER, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(4, 0));
            Assert.AreEqual(FieldOptions.COP, _model.GameTable.GetValue(5, 1));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(0, 5));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(1, 5));
            Assert.AreEqual(FieldOptions.TREE, _model.GameTable.GetValue(2, 5));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 3));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 4));
            Assert.AreEqual(FieldOptions.BASKET, _model.GameTable.GetValue(5, 5));

            //ellen�rizz�k, hogy a list�k rendben vannak-e
            Assert.AreEqual(2, _model.CopPosition.Count);
            Assert.AreEqual(3, _model.TreePosition.Count);
            Assert.AreEqual(3, _model.BasketPosition.Count);

            //ellen�rizz�k a kos�rsz�mot
            Assert.AreEqual(3, _model.BasketNumber);

            //ellen�rizz�k, hogy megh�vt�k-e a Load m�veletet
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

            //megh�vjuk a ment�st
            _model.SaveGame(String.Empty);

            //ellen�rizz�k, hogy nem v�ltoztak-e az �rt�kek ment�s ut�n
            Assert.AreEqual(_model.BasketNumber, collectableBaskets);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Assert.AreEqual(values[i, j], _model.GameTable.GetValue(i, j));
                }
            }

            // ellen�rizz�k, hogy megh�vt�k-e a Save m�veletet
            _mockFileManager.Verify(mock => mock.Save(String.Empty, It.IsAny<Table>()), Times.Once());
        }
    }
}
import System
from System import Console

class BaseFixture(NUnitFixture):	
	def testInBase(self):
		Console.WriteLine("--testInBase--")
		Assert.IsTrue(True)

class DerivedFixture(BaseFixture):	
	def testPass(self):
		Console.WriteLine("--testPass--")
		Assert.IsTrue(True)
	
	def testFail(self):
		Console.WriteLine("--testFail--")
		Assert.IsTrue(False, "this will fail")

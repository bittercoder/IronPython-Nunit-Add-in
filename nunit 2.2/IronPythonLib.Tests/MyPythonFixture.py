import System
from System import NullReferenceException

class MyPythonFixture(NUnitFixture):	
	def testPass(self):
		Console.WriteLine("--testPass--")
		Assert.IsTrue(True)
	
	def testFail(self):
		Console.WriteLine("--testFail--")
		Assert.IsTrue(False, "this will fail")
	
	def testFailUnlessRaised(self):
		def raiseNullRefEx():
			raise NullReferenceException()
		self.failUnlessRaises(NullReferenceException, raiseNullRefEx)
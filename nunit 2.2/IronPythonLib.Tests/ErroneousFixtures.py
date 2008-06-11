import System
from System import Console

class FailingSetup(NUnitFixture):	
	def setUp(self):
		raise StandardError
	
	def testSomething(self):
		Assert.IsTrue(True)

class FailingTeardown(NUnitFixture):	
	def tearDown(self):
		raise StandardError
	
	def testSomething(self):
		Assert.IsTrue(True)

class FailingSetupFixture(NUnitFixture):	
	def setUpFixture(self):
		raise StandardError
	
	def testSomething(self):
		Assert.IsTrue(True)

class FailingTeardownFixture(NUnitFixture):	
	def tearDownFixture(self):
		raise StandardError
	
	def testSomething(self):
		Assert.IsTrue(True)

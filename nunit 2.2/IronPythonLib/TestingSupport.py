import System
from System import Console

class NUnitFixture:
	def __init__(self):
		pass	
	def setUp(self):
		#Console.WriteLine("--setup--")
		pass		
	def tearDown(self):
		#Console.WriteLine("--teardown--")
		pass
	def setUpFixture(self):
		#Console.WriteLine("--setupFixture--")
		pass
	def tearDownFixture(self):
		#Console.WriteLine("--teardownFixture--")
		pass
	def failUnlessRaises(self, excClass, callableObj, *args, **kwargs):		
		if issubclass(excClass, System.Exception):
			try:
				callableObj(*args, **kwargs)
			except Exception, e:				
				if hasattr(e, "clsException") and (type(e.clsException) == excClass):
					return
		else:
			try:
				callableObj(*args, **kwargs)
			except excClass:
				return
		if hasattr(excClass,'__name__'):
			excName = excClass.__name__
		else: 
			excName = str(excClass)				
		raise AssertionException("%s not raised" % excName)

def getTestCaseNames(testCaseClass, prefix):
	def isTestMethod(attrname, testCaseClass=testCaseClass, prefix=prefix):
		return attrname.startswith(prefix) and callable(getattr(testCaseClass, attrname))
	testFnNames = filter(isTestMethod, dir(testCaseClass))
	for baseclass in testCaseClass.__bases__:
		for testFnName in getTestCaseNames(baseclass, prefix):
			if testFnName not in testFnNames:
				testFnNames.append(testFnName)
	testFnNames.sort(cmp)
	return testFnNames
#include "SFNetworkOscillator.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>
#include <fstream>
#include <thread>
#include <Windows.h>
#include <string>
#include <iomanip>
#include <sstream>
#include <ShObjIdl.h>


#define PI	3.1415926535897932384626433832795


using namespace std;
using namespace Diploma2;


int fileExists(char * file)
{
	size_t newsize = strlen(file) + 1;
	wchar_t * wcstring = new wchar_t[newsize];
	size_t convertedChars = 0;
	mbstowcs_s(&convertedChars, wcstring, newsize, file, _TRUNCATE);

	WIN32_FIND_DATA FindFileData;
	HANDLE handle = FindFirstFile(wcstring, &FindFileData);
	int found = handle != INVALID_HANDLE_VALUE;
	if (found)
	{
		//FindClose(&handle); this will crash
		FindClose(handle);
	}
	return found;
}

double measureRuntime(const function<void()> f) {
	clock_t start(clock());
	f();
	return (clock() - start) / (double)CLOCKS_PER_SEC;
}

bool stop = false;
bool binarized = false;
void cli() {
	while (!binarized) {
		string line;
		getline(cin, line);
		stop = line == "stop";
	}
}

template <typename T>
string to_string_with_precision(const T a_value, const int n = 6)
{
	ostringstream out;
	out << setprecision(n) << a_value;
	return out.str();
}
string network_name(SFNetworkOscillator& nw) {
	return "network_" + to_string(nw.node_count)
		+ "_" + to_string_with_precision(nw.multiplier, 0)
		+ "_" + to_string_with_precision(nw.strength, 3)
		+ "_" + to_string_with_precision(nw.freq_init_min, 3)
		+ "_" + to_string_with_precision(nw.freq_init_max, 3)
		+ "_" + to_string_with_precision(nw.phase_init_min, 3)
		+ "_" + to_string_with_precision(nw.phase_init_max, 3)
		+ "_" + to_string_with_precision(nw.time_init, 3)
		+ "_" + to_string_with_precision(nw.time_step, 3)
		+ "_" + to_string_with_precision(nw.solve_step, 3)
		+ "_" + to_string_with_precision(nw.seed, 0);
}



void hide_task_icon() {
	ITaskbarList *pTaskList = NULL;
	HRESULT initRet = CoInitialize(NULL);
	HRESULT createRet = CoCreateInstance(CLSID_TaskbarList,
		NULL,
		CLSCTX_INPROC_SERVER,
		IID_ITaskbarList,
		(LPVOID*)&pTaskList);

	if (createRet == S_OK)
	{

		pTaskList->DeleteTab(GetConsoleWindow());

		pTaskList->Release();
	}

	CoUninitialize();
}

void foo(SFNetworkOscillator &nw, string &file_name) {
	const long binary_timer_delay = 60000;

	long start_local = clock();
	cout << "Network generated in " << clock() - start_local << "ms" << endl;

	long binary_timer = clock();
	while (!stop) {

		start_local = clock();
		nw.SimulateDynamicStep();
		cout << "Dynamic:\ttime: " << nw.time << "\t" << clock() - start_local << "ms" << endl;
		
		if (stop) {
			binarized = true;
			binary_timer = -binary_timer_delay;
		}

		if (clock() - binary_timer > binary_timer_delay) {
			nw.Binarize(file_name.data(), 1);
			binary_timer = clock();
		}
	}
	cout << "Network generation stopped. Network successfully stored in file: " << file_name << endl;
}

double start = clock();

int main(int argc, char** argv) {
	/*SFNetworkOscillator ntw = SFNetworkOscillator(50, 3, 0.5, 1, 10, -PI, PI, 0, 0.1, 0.05, 147);
	ntw.SimulateDynamicStep();
	ntw.SimulateDynamicStep();
	ntw.SimulateDynamicStep();
	ntw.SimulateDynamicStep();
	ntw.SimulateDynamicStep();
	cout << ntw.multiplier;

	return 0;*/
	if (argc < 7 && argc != 2) {
		cout << "Argument list:\n\tnode_count\n\tmlt\n\tlambda\n\ttime_step\n\tsolve_step\n\trnd_seed\n\tfilename" << endl;
		return 0;
	}

	SFNetworkOscillator nw;

	if (argc == 2) {
		if (!fileExists(argv[1])) {
			cout << "Error while loading network from: " << argv[1] << endl;
			return 1;
		}
		nw.initialize(argv[1]);
	}
	else {

		double node_count = atof(argv[1]),
			mlt = atof(argv[2]),
			lambda = atof(argv[3]),
			time_step = atof(argv[4]),
			solve_step = atof(argv[5]),
			seed = atof(argv[6]);

		nw.initialize(node_count, mlt, lambda, 1, 10, -PI, PI, 0, time_step, solve_step, seed);
	}

	//hide_task_icon();
	srand(start);

	string file_name = network_name(nw) + ".bin";
	if (argc > 7) file_name = argv[7];

	double runtime = measureRuntime([nw, file_name](){


		thread calculation(&foo, nw, file_name);
		thread cli(&cli);
		cli.join();
		calculation.join();

	});

	cout << endl << "Runtime: " << runtime << endl;
	system("pause");
	return 0;
}
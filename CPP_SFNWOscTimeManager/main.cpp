#include "SFNetworkOscillator.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>
//#include <fstream>
#include <thread>
#include <Windows.h>
#include <string>
#include <iomanip>
#include <sstream>
#include <ShObjIdl.h>


#define PI	3.1415926535897932384626433832795
#define O(n, m, k)	n * n * n * m * k

#define DEFAULT_FILE_NAME	"queue.dll"


using namespace std;
using namespace Diploma2;


int fileExists(const char * file)
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
unsigned int split(const string &txt, vector<string> &strs, char ch)
{
	strs.clear();
	string t = "";
	for (int i = 0; i < txt.length(); i++) {
		if (txt[i] == ch) {
			strs.push_back(t);
			t = "";
			continue;
		}
		t += txt[i];
	}
	if (t.length() > 0) strs.push_back(t);
	return strs.size();
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



double predict(int n, int m, double ts, double ss) {
	const int times = 5;
	SFNetworkOscillator nw = SFNetworkOscillator(n, m, 0, 0, 1, 0, 1, 0, ss, ss);
	double N = ts / ss;
	if (N > times) N = times;
	double start = clock();
	for (int i = 0; i < N; i++)
		nw.SimulateDynamicStep();
	double t = (clock() - start) / N;
	return t * ts / ss;
}




int exec_counter = 0;
bool stop_current_execution = false;
bool cli_end_flag = false;
bool stop_whole_execution = false;

void cli() {
	while (!cli_end_flag) {
		string line;
		getline(cin, line);
		if (line == "stop") {
			stop_current_execution = true;
			stop_whole_execution = true;
		}
		if (line == "next") {
			stop_current_execution = true;
		}
	}
}
void nwcalc(SFNetworkOscillator &nw, string &file_name, double time_end) {
	const long binary_timer_delay = 60000;

	long binary_timer = clock();
	long start_local = 0;
	while (!stop_current_execution) {

		if (nw.time >= time_end) {
			stop_current_execution = true;
		}
		else {
			start_local = clock();
			nw.SimulateDynamicStep();
			cout << "#" << exec_counter << "\tTime: " << nw.time << "\t" << clock() - start_local << "ms" << endl;
		}

		if (stop_current_execution) {
			binary_timer = -binary_timer_delay;
		}

		if (clock() - binary_timer > binary_timer_delay) {
			nw.Binarize(file_name.data(), 1);
			binary_timer = clock();
		}
	}
	cout << "Network generation stopped. Network successfully stored in file: " << file_name << endl;
}

int execution(vector<string> argv, fstream &ofstr) {

	SFNetworkOscillator nw;
	string file_name;
	double time_end = -1;

	if (argv.size() == 0 || argv[0] == "#") return 0;
	if (argv[0] == "_") {
		ShowWindow(GetConsoleWindow(), SW_MINIMIZE);
		return 0;
	}
	if (argv[0] == "^") {
		ShowWindow(GetConsoleWindow(), SW_NORMAL);
		return 0;
	}
	if (argv[0] == "*") {
		ShowWindow(GetConsoleWindow(), SW_HIDE);
		return 0;
	}

	time_t now = time(0);
	char* dt = ctime(&now);
	ofstr << "Execution #" << exec_counter << " started at " << dt << endl;

	cout << "Processing network (command #" << exec_counter << "):" << endl;
	for (string i : argv) {
		cout << "\t" << i << endl;
	}
	cout << endl;

	double start_time_execution = clock();


	if (argv.size() == 1) {
		cout << "No option with 1 argument available" << endl;
		return 1;
	}
	if (argv.size() == 2) {

		if (!fileExists(argv[0].data())) {
			cout << "File '" << argv[0] << "' does not exist" << endl;
			return 2;
		}

		try {
			nw.initialize(argv[0]);
			file_name = argv[0];
			time_end = atof(argv[1].data());
		}
		catch (exception e) {
			cout << "Error occured while loading network from file '" << argv[0] << "'" << endl << "\t" << e.what() << endl;
			return 3;
		}
	}
	else {
		try {
			int node_count = atof(argv[0].data()),
				mlt = atof(argv[1].data());
			double lambda = atof(argv[2].data()),
				time_step = atof(argv[3].data()),
				solve_step = atof(argv[4].data());
			time_end = atof(argv[5].data());
			double seed = rand();
			if (argv.size() >= 7)
				if (argv[6] != "-")
					seed = atoi(argv[6].data());

			nw.initialize(node_count, mlt, lambda, 1, 10, -PI, PI, 0, time_step, solve_step, seed);
			file_name = network_name(nw) + ".bin";

			if (argv.size() >= 8)
				file_name = argv[7];

			if (fileExists(file_name.data())) {
				try {
					nw.initialize(file_name);
				}
				catch (exception e) {
					cout << "Error while loading network from '" << file_name << "'" << endl;
					return 5;
				}
			}

		}
		catch (exception e) {
			cout << "Error occured while initializing network" << endl << "\t" << e.what() << endl;
			return 4;
		}
	}

	//cout << "Prediction for each time iteration is " << predict(nw.node_count, nw.multiplier, nw.time_step, nw.solve_step) << endl;

	stop_current_execution = false;
	thread calculation(&nwcalc, ref(nw), ref(file_name), time_end);
	calculation.join();

	cout << "Execution #" << exec_counter << " finished in " << clock() - start_time_execution << "ms" << endl << endl;
	exec_counter++;

	return 0;
}

long end_global = 0;
void cmds_exec(vector<vector<string>> &commands, fstream &ofstr) {
	for (vector<string> vec : commands) {
		execution(vec, ofstr);
		if (stop_whole_execution) {
			cout << "The whole execution terminated by user" << endl;
			break;
		}
	}
	cli_end_flag = true;
	end_global = clock();
	cout << "Execution of the whole command list finished. Press any key to continue." << endl;
}



int main(int argc, char** argv) {

	srand(clock());
	ShowWindow(GetConsoleWindow(), SW_MINIMIZE);


	time_t now = time(0);
	char* dt = ctime(&now);
	cout << "The local date and time is: " << dt << endl;

	fstream ofstr = fstream("logs.txt", ios::out | ios::app);
	ofstr << "App started at " << dt << endl;


	long start_global = clock();

	vector<string> args2vec(argc - 1);
	for (int i = 1; i < argc; i++) args2vec[i - 1] = argv[i];
	if (argc > 2) {
		cout << "Arguments for the only network received" << endl << endl;
		return execution(args2vec, ofstr);
	}

	string file_name = DEFAULT_FILE_NAME;
	if (argc == 2)
		if (fileExists(argv[1]))
			file_name = argv[1];

	if (!fileExists(file_name.data())) {
		cout << "File '" << file_name << "' was not found" << endl;
		return 1;
	}

	fstream ifstr = fstream(file_name.data(), ios::in);
	vector<string> args(0);

	while (!ifstr.eof()) {
		args.push_back("");
		getline(ifstr, args[args.size() - 1]);
	}
	ifstr.close();

	vector<vector<string>> commands(0);

	cout << "The following command list has been loaded:" << endl;
	for (int i = 0; i < args.size(); i++)
	{
		cout << i << ".\t" << args[i] << endl;
		vector<string> arg;
		split(args[i], arg, ' ');
		commands.push_back(arg);
	}
	cout << endl;

	thread cli(&cli);
	thread cmds_execution(&cmds_exec, ref(commands), ref(ofstr));
	cli.join();
	cmds_execution.join();


	now = time(0);
	dt = ctime(&now);
	ofstr << "Application ended at " << dt << endl;
	ofstr.close();

	cout << "Execution of the whole command list finished in " << end_global - start_global << endl;
	system("pause");
	return 0;
}
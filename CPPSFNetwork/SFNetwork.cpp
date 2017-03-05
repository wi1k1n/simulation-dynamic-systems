#include "SFNetwork.h"
#include "ilRand.h"

#include <vector>
#include <algorithm>
#include <random>
#include <fstream>


#define PI2				6.283185307179586476925286766559



Edge::Edge() {}
Edge::Edge(int f, int t, int w) {
	from = f;
	to = t;
	weight = w;
}
Edge::~Edge()
{
}
void Edge::Binarize(std::ofstream &str) {
	str.write((char*)&from, sizeof(int));
	str.write((char*)&to, sizeof(int));
	str.write((char*)&weight, sizeof(int));
}




void SFNetwork::generate() {
	int m = node_count * multiplier,
		l = 2 * m;
	std::vector<int> alphabet(l);
	for (int i = 0; i < l; i++) alphabet[i] = i;
	std::vector<int> lcd(l, -1);
	for (int i = 0; i < m; i++)
	{
		int i1 = ilRandom.Next(0, alphabet.size()),
			r1 = alphabet[i1];
		alphabet.erase(alphabet.begin() + i1);
		int i2 = ilRandom.Next(0, alphabet.size()),
			r2 = alphabet[i2];
		alphabet.erase(alphabet.begin() + i2);
		lcd[std::max(r1, r2)] = std::min(r1, r2);
	}
	std::vector<int> node_marker(l);
	int kCnt = 0,
		globalNode = 0;
	for (int i = 0; i < l; i++)
	{
		node_marker[i] = globalNode;
		if (lcd[i] > -1 && ++kCnt == 3)
		{
			globalNode++;
			kCnt = 0;
		}
	}
	edges = std::vector<Edge>(0);
	bool f = false;
	int j = 0;
	for (int i = 0; i < l; i++)
	{
		if (lcd[i] == -1) continue;
		Edge edg(node_marker[i], node_marker[lcd[i]], 1);
		f = false;
		for (j = 0, f = false; j < edges.size(); j++)
			if (edges[j].from == edg.from && edges[j].to == edg.to) {
				f = true;
				break;
			}
		if (!f) edges.push_back(edg);
		else edges[j].weight++;
	}
}
SFNetwork::SFNetwork(int nodes, int mlt)
{
	ilRandom.Initialize();
	seed = ilRandom.seed;
	node_count = nodes;
	multiplier = mlt;
	generate();
}
SFNetwork::SFNetwork(int nodes, int mlt, int s)
{
	ilRandom.Initialize(s);
	seed = ilRandom.seed;
	node_count = nodes;
	multiplier = mlt;
	generate();
}
SFNetwork::~SFNetwork()
{
}




SFNetworkOscillatorState::SFNetworkOscillatorState(double t, std::vector<double> p) {
	time = t;
	phases = p;
}
SFNetworkOscillatorState::~SFNetworkOscillatorState() {};

void SFNetworkOscillatorState::Binarize(std::ofstream &str) {
	str.write((char*)&time, sizeof(double));

	int phase_count = phases.size();
	str.write((char*)&phase_count, sizeof(int));
	for (double ph : phases)
		str.write((char*)&ph, sizeof(double));
}



void SFNetworkOscillator::constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step) {
	strength = str;
	freq_init_min = f_min;
	freq_init_max = f_max;
	phase_init_min = p_min;
	phase_init_max = p_max;
	time_init = t_init;
	time = t_init;
	time_step = t_step;
	solve_step = s_step;

	freqs = std::vector<double>(node_count);
	phases = std::vector<double>(node_count);
	for (int i = 0; i < node_count; i++)
	{
		freqs[i] = ilRandom.Next((int)f_min, (int)f_max);
		phases[i] = ilRandom.NextDouble(p_min, p_max);
	}
	states = std::vector<SFNetworkOscillatorState>{ SFNetworkOscillatorState(time, phases) };
	funcs = std::vector<RK4SFunc>(node_count);
	for (int i = 0; i < funcs.size(); i++) {
		funcs[i] = [this](int ind, double t, const std::vector<double>& x)->double {
			double res = 0;
			for (int j = 0; j < x.size(); j++)
			{
				//if (j == ind) continue;
				int w = 0;
				for (Edge edg : this->edges) {
					if (edg.from == j && edg.to == ind || edg.from == ind && edg.to == j)
					{
						w += edg.weight;
						break;
					}
				}
				res += w * sin(x[j] - x[ind]);
			}
			return phases[ind] + strength * res;
		};
	}
}
SFNetworkOscillator::SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step) : SFNetwork(node_count, mlt) {
	constructor(strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step);
}
SFNetworkOscillator::SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed) : SFNetwork(node_count, mlt, random_seed) {
	constructor(strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step);
}
SFNetworkOscillator::~SFNetworkOscillator() {};

void SFNetworkOscillator::phasesNormalize() {
	for (int i = 0; i < phases.size(); i++)
		phases[i] = phases[i] - (int)(phases[i] / PI2) * PI2;
}
void SFNetworkOscillator::SimulateDynamicStep() {
	RK4SResult result = RK4S::Solve(funcs, time, phases, time + time_step, solve_step);
	time = result.time;
	phases = result.values;
	phasesNormalize();
	states.push_back(SFNetworkOscillatorState(time, phases));
}

void SFNetworkOscillator::Binarize(const char* path, char version) {
	switch (version)
	{
		case 1:
			Binarize_v1(path); break;
		default: break;
	}
}
void SFNetworkOscillator::Binarize_v1(const char* path) {
	std::ofstream ofile(path, std::ios::binary);
	char version = 1;
	ofile.write(&version, sizeof(char));
	ofile.write((char*)&node_count, sizeof(int));
	ofile.write((char*)&multiplier, sizeof(int));
	ofile.write((char*)&seed, sizeof(int));
	ofile.write((char*)&ilRandom.x, sizeof(long long));
	int edge_count = edges.size();
	ofile.write((char*)&edge_count, sizeof(int));
	for (Edge edg : edges)
		edg.Binarize(ofile);

	ofile.write((char*)&strength, sizeof(double));
	ofile.write((char*)&freq_init_min, sizeof(double));
	ofile.write((char*)&freq_init_max, sizeof(double));
	ofile.write((char*)&phase_init_min, sizeof(double));
	ofile.write((char*)&phase_init_max, sizeof(double));
	ofile.write((char*)&time_init, sizeof(double));
	ofile.write((char*)&time_step, sizeof(double));
	ofile.write((char*)&solve_step, sizeof(double));
	ofile.write((char*)&time, sizeof(double));

	int phase_count = phases.size();
	ofile.write((char*)&phase_count, sizeof(int));
	for (double ph : phases)
		ofile.write((char*)&ph, sizeof(double));

	int freq_count = freqs.size();
	ofile.write((char*)&freq_count, sizeof(int));
	for (double fr : freqs)
		ofile.write((char*)&fr, sizeof(double));

	int state_count = states.size();
	ofile.write((char*)&state_count, sizeof(int));
	for (SFNetworkOscillatorState st : states)
		st.Binarize(ofile);
}
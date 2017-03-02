#include "SFNetwork.h"

#include <vector>
#include <algorithm>
#include <random>


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




SFNetwork::SFNetwork(int nodes, int mlt)
{
	node_count = nodes;
	multiplier = mlt;

	int m = node_count * mlt,
		l = 2 * m;
	std::vector<int> alphabet(l);
	for (int i = 0; i < l; i++) alphabet[i] = i;
	std::vector<int> lcd(l, -1);
	for (int i = 0; i < m; i++)
	{
		int i1 = random(alphabet.size(), 0),
			r1 = alphabet[i1];
		alphabet.erase(alphabet.begin() + i1);
		int i2 = random(alphabet.size(), 0),
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
SFNetwork::~SFNetwork()
{
}
int SFNetwork::random(int max = RAND_MAX, int min = 0) {
	return min + rand() % max;
}
double SFNetwork::random(double max = RAND_MAX, double min = 0) {
	return std::uniform_real_distribution<double>(min, max)(std::default_random_engine(rand()));
}




SFNetworkOscillatorState::SFNetworkOscillatorState(double t, std::vector<double> p) {
	time = t;
	phases = p;
}
SFNetworkOscillatorState::~SFNetworkOscillatorState() {};

void SFNetworkOscillator::constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step, int seed) {
	strength = str;
	freq_init_min = f_min;
	freq_init_max = f_max;
	phase_init_min = p_min;
	phase_init_max = p_max;
	time_init = t_init;
	time = t_init;
	time_step = t_step;
	solve_step = s_step;
	random_seed = seed;

	srand(seed);

	freqs = std::vector<double>(node_count);
	phases = std::vector<double>(node_count);
	for (int i = 0; i < node_count; i++)
	{
		freqs[i] = random(f_max, f_min);
		phases[i] = random(p_max, p_min);
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
	constructor(strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step, rand());
}
SFNetworkOscillator::SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed) : SFNetwork(node_count, mlt) {
	constructor(strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step, random_seed);
}
SFNetworkOscillator::~SFNetworkOscillator() {};

void SFNetworkOscillator::phasesNormalize() {
	for (int i = 0; i < phases.size(); i++)
		if (phases[i] > PI2)
			phases[i] = phases[i] - (int)(phases[i] / PI2) * PI2;
}
void SFNetworkOscillator::SimulateDynamicStep() {
	RK4SResult result = RK4S::Solve(funcs, time, phases, time + time_step, solve_step);
	time = result.time;
	phases = result.values;
	phasesNormalize();
	states.push_back(SFNetworkOscillatorState(time, phases));
}
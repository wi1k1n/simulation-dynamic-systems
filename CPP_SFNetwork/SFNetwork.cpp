#include "SFNetwork.h"
#include "ilRand.h"

#include <vector>
#include <algorithm>
#include <random>
#include <fstream>


#define PI2				6.283185307179586476925286766559


namespace Diploma2 {

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
		str.write((char*)&from, sizeof(from));
		str.write((char*)&to, sizeof(to));
		str.write((char*)&weight, sizeof(weight));
	}
	Edge Edge::Debinarize(std::ifstream &str) {
		int b[3];
		str.read(reinterpret_cast<char *>(&b), sizeof(b));
		return Edge(b[0], b[1], b[2]);
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
			if (lcd[i] > -1 && ++kCnt == multiplier)
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
	SFNetwork::SFNetwork() {}
	SFNetwork::SFNetwork(int nodes, int mlt)
	{
		initialize_sfnw(nodes, mlt);
	}
	SFNetwork::SFNetwork(int nodes, int mlt, int s)
	{
		initialize_sfnw(nodes, mlt, s);
	}
	SFNetwork::SFNetwork(int nodes, int mlt, std::vector<Edge> &edgs, ilRand &rnd)
	{
		initialize_sfnw(nodes, mlt, edgs, rnd);
	}
	SFNetwork::~SFNetwork()
	{
	}


	void SFNetwork::initialize_sfnw(int nodes, int mlt)
	{
		ilRandom.Initialize();
		seed = ilRandom.seed;
		node_count = nodes;
		multiplier = mlt;
		generate();
	}
	void SFNetwork::initialize_sfnw(int nodes, int mlt, int s)
	{
		ilRandom.Initialize(s);
		seed = ilRandom.seed;
		node_count = nodes;
		multiplier = mlt;
		generate();
	}
	void SFNetwork::initialize_sfnw(int nodes, int mlt, std::vector<Edge> &edgs, ilRand &rnd)
	{
		ilRandom.Initialize(rnd.seed, rnd.x);
		edges = edgs;
		seed = ilRandom.seed;
		node_count = nodes;
		multiplier = mlt;
	}


}
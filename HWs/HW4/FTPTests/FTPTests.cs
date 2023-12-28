namespace FTPTests;

using System.Net;
using System.Text;
using FTPClient;
using FTPServer;

public class Tests
{
    private FTPServer _server;
    private FTPClient _client;
    private int _port = 8888;
    private string _address = "127.0.0.1";

    [SetUp]
    public async Task Setup()
    {
        _server = new FTPServer(_port);
        _client = new FTPClient(_address, _port);

        _ = Task.Run(() => _server.StartAsync());
        await Task.Delay(100);
    }

    [TearDown]
    public void TearDown()
    {
        _server.Stop();
        Task.Delay(100);
    }

    [Test]
    public async Task ListAsync_ValidPath_ReturnsCorrectData()
    {
        var actual = await _client.ListAsync("../../../Files");
        var expected = "2 empty.txt False Folder True";

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAsync_ValidPath_ReturnsCorrectData()
    {
        var response = await _client.GetAsync("../../../Files/Folder/file.txt");
        if (response is null)
        {
            Assert.Fail();
            return;
        }

        var parts = response.Split(' ');

        var actualCount = parts[0];
        var actualContent = Encoding.UTF8.GetString(Enumerable.Range(0, parts[1].Length)
                                                 .Where(x => x % 2 == 0)
                                                 .Select(x => Convert.ToByte(parts[1].Substring(x, 2), 16))
                                                 .ToArray());

        var expectedCount = "13";
        var expectedContent = "Hello, world!";

        Assert.Multiple(() =>
        {
            Assert.That(actualCount, Is.EqualTo(expectedCount));
            Assert.That(actualContent, Is.EqualTo(expectedContent));
        });
    }

    [Test]
    public async Task ListAsync_InvalidPath_ReturnsMinusOne()
    {
        var actual = await _client.ListAsync("../../../InvalidPath");
        var expected = "-1";

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAsync_InvalidPath_ReturnsMinusOne()
    {
        var actual = await _client.GetAsync("../../../InvalidPath");
        var expected = "-1";

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAsync_EmptyFile_ReturnsZero()
    {

        var actual = await _client.GetAsync("../../../Files/empty.txt");
        var expected = "0 ";

        Assert.That(actual, Is.EqualTo(expected));
    }
}
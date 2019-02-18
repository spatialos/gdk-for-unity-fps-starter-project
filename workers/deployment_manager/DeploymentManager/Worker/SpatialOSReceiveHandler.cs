using Improbable.Worker;
using Deployment = Improbable.SpatialOS.Deployment.V1Alpha1.Deployment;

namespace DeploymentManager
{
    public class SpatialOSReceiveHandler
    {
        private readonly Dispatcher dispatcher = new Dispatcher();
        private readonly Connection connection;
        private readonly string deploymentId;
        private readonly string projectName;

        public SpatialOSReceiveHandler(Connection connection, string deploymentId, string projectName)
        {
            this.connection = connection;
            this.deploymentId = deploymentId;
            this.projectName = projectName;
            SetupHandlers();
        }

        public void ProcessOps()
        {
            using (var opList = connection.GetOpList(0))
            {
                dispatcher.Process(opList);
            }
        }

        private void SetupHandlers()
        {
            dispatcher.OnLogMessage(OnLogMessage);
            dispatcher.OnAddComponent<Improbable.Session.Session>(OnComponentAdded);
            dispatcher.OnComponentUpdate<Improbable.Session.Session>(OnComponentUpdate);
        }

        private void OnLogMessage(LogMessageOp message)
        {
            Log.Print(message.Level, message.Message, connection);
        }


        private void OnComponentAdded(AddComponentOp<Improbable.Session.Session> componentAddOp)
        {
            Improbable.Session.Session.Data data = (Improbable.Session.Session.Data) componentAddOp.Data;
            var status = data.Value.status;

            DeploymentModifier.UpdateDeploymentTag(deploymentId, projectName, "status", status.ToString().ToLower());

            if (status == Improbable.Session.Status.STOPPED)
            {
                var deployment = DeploymentModifier.GetDeployment(deploymentId, projectName);
                DeploymentModifier.StopDeployment(deployment);
            }
        }

        private void OnComponentUpdate(ComponentUpdateOp<Improbable.Session.Session> componentUpdate)
        {
            Improbable.Session.Session.Update update = (Improbable.Session.Session.Update) componentUpdate.Update;
            var status = update.status;

            DeploymentModifier.UpdateDeploymentTag(deploymentId, projectName, "status", status.ToString().ToLower());

            if (status == Improbable.Session.Status.STOPPED)
            {
                var deployment = DeploymentModifier.GetDeployment(deploymentId, projectName);
                DeploymentModifier.StopDeployment(deployment);
            }
        }
    }
}
